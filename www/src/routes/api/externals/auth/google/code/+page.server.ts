import { env } from '$env/dynamic/private';
import { CacheKey } from '$lib/utils/cache';
import {
    enrich,
    enrichStep,
    ErrorCode,
    GenericError,
    isError,
    mapFetchException,
    MismatchOAuthStateError,
    MissingGoogleAuthorizationCodeError,
    MissingOAuthStateError,
    ValidationError,
} from '$lib/utils/errors';
import { jsonify, parseHttpProblem } from '$lib/utils/http';
import { problemDetailsValidator } from '$lib/utils/problem';
import { attempt } from '@duydang2311/attempt';
import { error, redirect } from '@sveltejs/kit';
import { useRuntime } from '~/lib/utils/runtime.server';
import type { PageServerLoad } from './$types';
import type { HttpClient } from '~/lib/services/http_client';

export const load: PageServerLoad = async (e) => {
    const oauthState = e.cookies.get('oauth_state');
    if (!oauthState) {
        return error(400, MissingOAuthStateError());
    }
    e.cookies.delete('oauth_state', {
        path: '/api/externals/auth/google/code',
        httpOnly: true,
        secure: true,
        sameSite: 'lax',
    });

    if (oauthState !== e.url.searchParams.get('state')) {
        return error(400, MismatchOAuthStateError());
    }

    const code = e.url.searchParams.get('code');
    if (!code) {
        return error(400, MissingGoogleAuthorizationCodeError());
    }

    const exchanged = await attempt.async(() =>
        fetch('https://oauth2.googleapis.com/token', {
            method: 'post',
            body: new URLSearchParams({
                code,
                client_id: env.GOOGLE_OAUTH_CLIENT_ID,
                client_secret: env.GOOGLE_OAUTH_CLIENT_SECRET,
                grant_type: 'authorization_code',
                redirect_uri: `${e.url.origin}/api/externals/auth/google/code`,
            }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        })
    )((e) => enrich({ step: 'exchange_token' })(mapFetchException(e)));
    if (!exchanged.ok) {
        return error(500, exchanged.error);
    }

    if (!exchanged.data.ok) {
        return error(exchanged.data.status, GenericError(await exchanged.data.json()));
    }

    const parsed = await jsonify(() =>
        exchanged.data.json<{
            access_token: string;
            expires_in: number;
            scope: string;
            token_type: string;
            id_token: string;
        }>()
    );
    if (!parsed.ok) {
        return error(500, enrichStep('parse_google_response')(parsed.error));
    }

    const created = await createSession(e.locals.http)(parsed.data.id_token);
    if (
        !created.ok &&
        problemDetailsValidator.check(created.error) &&
        created.error.errors?.some(
            (a) =>
                (a.field === 'GoogleIdToken' && a.code === ErrorCode.NotFound) ||
                (a.field === '$' && a.code === ErrorCode.UserNotFound)
        )
    ) {
        const id = crypto.randomUUID();
        e.platform!.env.APP_KV.put(
            CacheKey.completeOAuthRegistration(id),
            JSON.stringify({
                provider: 'google',
                idToken: parsed.data.id_token,
            }),
            { expirationTtl: 60 * 5 }
        );
        e.cookies.set('oauth_complete_session', id, {
            path: '/sign-up/complete',
            httpOnly: true,
            secure: true,
            sameSite: 'lax',
            maxAge: 60 * 5,
        });
        return redirect(303, '/sign-up/complete');
    }

    if (!created.ok) {
        if (isError(created.error)) {
            return error(500, created.error);
        }
        return error(created.error.status, ValidationError.from(created.error));
    }

    const parsedCreatedResponse = await jsonify(() => created.data.json<{ token: string }>());
    if (!parsedCreatedResponse.ok) {
        return error(500, enrichStep('parse_create_session_response')(parsedCreatedResponse.error));
    }
    e.cookies.set('session_token', parsedCreatedResponse.data.token, {
        path: '/',
        maxAge: 60 * 60 * 24 * 7,
        httpOnly: true,
        secure: true,
        sameSite: 'lax',
    });
    return redirect(303, '/');
};

const createSession = (http: HttpClient) => async (googleIdToken: string) => {
    const created = await http.post('sessions', {
        body: {
            provider: 'google',
            googleIdToken,
        },
    });
    if (!created.ok) {
        return attempt.fail(enrichStep('create_session')(created.error));
    }

    if (!created.data.ok) {
        const parsedJson = await jsonify(() => created.data.json());
        if (!parsedJson.ok) {
            return attempt.fail(enrichStep('parse_response_json')(parsedJson.error));
        }
        const parsedProblem = parseHttpProblem(parsedJson.data);
        if (!parsedProblem.ok) {
            return attempt.fail(enrichStep('parse_problem')(GenericError(parsedJson.data)));
        }
        return attempt.fail(parsedProblem.data);
    }
    return created;
};
