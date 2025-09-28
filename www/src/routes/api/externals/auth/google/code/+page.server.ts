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
import { useRuntime } from '$lib/utils/runtime';
import pipe from '@bitty/pipe';
import { attempt } from '@duydang2311/attempt';
import { error, redirect } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ cookies, url }) => {
	const oauthState = cookies.get('oauth_state');
	if (!oauthState) {
		return error(400, MissingOAuthStateError());
	}
	cookies.delete('oauth_state', {
		path: '/api/externals/auth/google/code',
		httpOnly: true,
		secure: true,
		sameSite: 'lax',
	});

	if (oauthState !== url.searchParams.get('state')) {
		return error(400, MismatchOAuthStateError());
	}

	const code = url.searchParams.get('code');
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
				redirect_uri: `${url.origin}/api/externals/auth/google/code`,
			}),
			headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
		})
	)(pipe(mapFetchException, enrich({ step: 'exchange_token' })));
	if (exchanged.failed) {
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
	if (parsed.failed) {
		return error(500, enrichStep('parse_google_response')(parsed.error));
	}

	const created = await createSession(parsed.data.id_token);
	if (
		created.failed &&
		problemDetailsValidator.check(created.error) &&
		created.error.errors?.some(
			(a) =>
				(a.field === 'GoogleIdToken' && a.code === ErrorCode.NotFound) ||
				(a.field === '$' && a.code === ErrorCode.UserNotFound)
		)
	) {
		const id = crypto.randomUUID();
		const { cache } = useRuntime();
		await cache.set(
			CacheKey.completeOAuthRegistration(id),
			{
				provider: 'google',
				idToken: parsed.data.id_token,
			},
			'5m'
		);
		cookies.set('oauth_complete_session', id, {
			path: '/sign-up/complete',
			httpOnly: true,
			secure: true,
			sameSite: 'lax',
			maxAge: 60 * 5,
		});
		return redirect(303, '/sign-up/complete');
	}

	console.log('created', created);
	if (created.failed) {
		if (isError(created.error)) {
			return error(500, created.error);
		}
		return error(created.error.status, ValidationError.from(created.error));
	}

	const parsedCreatedResponse = await jsonify(() => created.data.json<{ token: string }>());
	if (parsedCreatedResponse.failed) {
		return error(500, enrichStep('parse_create_session_response')(parsedCreatedResponse.error));
	}
	cookies.set('session_token', parsedCreatedResponse.data.token, {
		path: '/',
		maxAge: 60 * 60 * 24 * 7,
		httpOnly: true,
		secure: true,
		sameSite: 'lax',
	});
	return redirect(303, '/');
};

const createSession = async (googleIdToken: string) => {
	const { http } = useRuntime();
	const created = await http.post('sessions', {
		body: {
			provider: 'google',
			googleIdToken,
		},
	});
	if (created.failed) {
		return attempt.fail(enrichStep('create_session')(created.error));
	}

	if (!created.data.ok) {
		const parsed = await parseHttpProblem(created.data);
		if (parsed.failed) {
			return attempt.fail(enrichStep('parse_create_session_http_problem')(parsed.error));
		}
		return attempt.fail(parsed.data);
	}
	return created;
};
