import { env } from '$env/dynamic/private';
import { redirect } from '@sveltejs/kit';
import * as openIdClient from 'openid-client';
import { withObservability } from '~/lib/utils/observability';
import type { RequestHandler } from './$types';

export const GET = withObservability((async (e) => {
    const server = new URL('https://accounts.google.com/.well-known/openid-configuration');
    const clientId = env.GOOGLE_OAUTH_CLIENT_ID;
    const clientSecret = env.GOOGLE_OAUTH_CLIENT_SECRET;
    const redirect_uri = `${e.url.origin}/auth/google/callback`;
    const scope = 'openid email';
    const config = await openIdClient.discovery(server, clientId, clientSecret);
    const code_verifier = openIdClient.randomPKCECodeVerifier();
    e.cookies.set('oauth_code_verifier', code_verifier, {
        path: '/api/externals/auth/google/code',
        httpOnly: true,
        secure: true,
        sameSite: 'lax',
        maxAge: 60,
    });
    const code_challenge = await openIdClient.calculatePKCECodeChallenge(code_verifier);
    const parameters: Record<string, string> = {
        redirect_uri,
        scope,
        code_challenge,
        code_challenge_method: 'S256',
    };

    if (!config.serverMetadata().supportsPKCE()) {
        const state = openIdClient.randomState();
        parameters.state = state;
        e.cookies.set('oauth_state', state, {
            path: '/api/externals/auth/google/code',
            httpOnly: true,
            secure: true,
            sameSite: 'lax',
            maxAge: 60,
        });
    }

    const authUrl = openIdClient.buildAuthorizationUrl(config, parameters);
    return redirect(303, authUrl);
}) satisfies RequestHandler);
