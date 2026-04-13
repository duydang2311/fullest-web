import { env } from '$env/dynamic/private';
import { CacheKey } from '$lib/utils/cache';
import { Err, ErrorCode } from '$lib/utils/errors';
import { jsonify, parseHttpError } from '$lib/utils/http';
import { problemDetailsValidator } from '$lib/utils/problem';
import { attempt } from '@duydang2311/attempt';
import { error, redirect } from '@sveltejs/kit';
import type { HttpClient } from '~/lib/services/http_client';
import { guardNull } from '~/lib/utils/guard';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
    const oauthState = e.cookies.get('oauth_state');
    if (!oauthState) {
        return error(400, Err('ERR_MISSING_OAUTH_STATE'));
    }
    e.cookies.delete('oauth_state', {
        path: '/api/externals/auth/google/code',
        httpOnly: true,
        secure: true,
        sameSite: 'lax',
    });

    if (oauthState !== e.url.searchParams.get('state')) {
        return error(400, Err('ERR_MISMATCH_OAUTH_STATE'));
    }

    const code = e.url.searchParams.get('code');
    if (!code) {
        return error(400, Err('ERR_MISSING_GOOGLE_AUTHORIZAITION_CODE'));
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
    )();
    if (!exchanged.ok) {
        return error(500, Err('ERR_EXCHANGE_TOKEN_FAILED'));
    }

    if (!exchanged.data.ok) {
        const err = await parseHttpError(exchanged.data);
        return error(err.status, err);
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
        return error(500, Err('ERR_PARSE_TOKEN_BODY_FAILED'));
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
        guardNull(e.platform);
        await e.platform.env.APP_KV.put(
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
        if (created.error.kind === 'HttpValidationError' || created.error.kind === 'HttpError') {
            return error(created.error.status, created.error);
        }
        return error(500, created.error);
    }

    const parsedCreatedResponse = await jsonify(() => created.data.json<{ token: string }>());
    if (!parsedCreatedResponse.ok) {
        return error(500, Err('ERR_PARSE_SESSION_BODY_FAILED'));
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
        return attempt.fail(Err('ERR_FETCH_SESSION'));
    }

    if (!created.data.ok) {
        const err = await parseHttpError(created.data);
        return attempt.fail(err);
    }
    return created;
};
