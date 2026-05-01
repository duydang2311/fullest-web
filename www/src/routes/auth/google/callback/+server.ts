import { env } from '$env/dynamic/private';
import { CacheKey } from '$lib/utils/cache';
import { Err, ErrorCode } from '$lib/utils/errors';
import { jsonify, parseHttpError } from '$lib/utils/http';
import { problemDetailsValidator } from '$lib/utils/problem';
import { attempt } from '@duydang2311/attempt';
import { error, redirect } from '@sveltejs/kit';
import * as openIdClient from 'openid-client';
import type { HttpClient } from '~/lib/services/http_client';
import { guardNull } from '~/lib/utils/guard';
import { withObservability } from '~/lib/utils/observability';
import type { RequestHandler } from './$types';

export const GET = withObservability((async (e) => {
    const oauthCodeVerifier =
        e.cookies.get('oauth_code_verifier') ?? error(400, Err('ERR_MISSING_OAUTH_CODE_VERIFIER'));
    const oauthState = e.cookies.get('oauth_state');
    if (oauthState) {
        e.cookies.delete('oauth_state', {
            path: '/auth/google/callback',
            httpOnly: true,
            secure: true,
            sameSite: 'lax',
        });
    }
    e.cookies.delete('oauth_code_verifier', {
        path: '/auth/google/callback',
        httpOnly: true,
        secure: true,
        sameSite: 'lax',
    });

    const server = new URL('https://accounts.google.com/.well-known/openid-configuration');
    const clientId = env.GOOGLE_OAUTH_CLIENT_ID;
    const clientSecret = env.GOOGLE_OAUTH_CLIENT_SECRET;
    const config = await openIdClient.discovery(server, clientId, clientSecret);
    const grantResp = await openIdClient.authorizationCodeGrant(config, e.request, {
        pkceCodeVerifier: oauthCodeVerifier,
        expectedState: oauthState,
    });

    if (!grantResp.id_token) {
        return error(400, Err('ERR_MISSING_ID_TOKEN'));
    }

    const created = await createSession(e.locals.http)(grantResp.id_token);
    if (
        !created.ok &&
        (created.error.kind === 'HttpError' || created.error.kind === 'HttpValidationError') &&
        created.error.status === 404
    ) {
        const id = crypto.randomUUID();
        guardNull(e.platform);
        await e.platform.env.APP_KV.put(
            CacheKey.completeOAuthRegistration(id),
            JSON.stringify({
                provider: 'google',
                idToken: grantResp.id_token,
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
}) satisfies RequestHandler);

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
