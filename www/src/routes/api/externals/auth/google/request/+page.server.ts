import { env } from '$env/dynamic/private';
import { InternalServerError } from '$lib/utils/errors';
import { attempt } from '@duydang2311/attempt';
import { error, redirect } from '@sveltejs/kit';
import { randomBytes } from 'node:crypto';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ url, cookies }) => {
	const createOAuthState = new Promise<string>((resolve, reject) => {
		randomBytes(32, (err, buf) => {
			if (err) {
				reject(err);
				return;
			}
			resolve(buf.toString('base64url'));
		});
	});
	const created = await attempt.async(() => createOAuthState)((e) => e as Error);
	if (!created.ok) {
		return error(500, InternalServerError(created.error.message));
	}

	cookies.set('oauth_state', created.data, {
		path: '/api/externals/auth/google/code',
		httpOnly: true,
		secure: true,
		sameSite: 'lax',
		maxAge: 60,
	});

	const oauthUrl = new URL('https://accounts.google.com/o/oauth2/v2/auth');
	oauthUrl.searchParams.set('client_id', env.GOOGLE_OAUTH_CLIENT_ID!);
	oauthUrl.searchParams.set('redirect_uri', `${url.origin}/api/externals/auth/google/code`);
	oauthUrl.searchParams.set('response_type', 'code');
	oauthUrl.searchParams.set('scope', 'openid email profile');
	oauthUrl.searchParams.set('state', created.data);
	oauthUrl.searchParams.set('include_granted_scopes', 'true');

	return redirect(303, oauthUrl);
};
