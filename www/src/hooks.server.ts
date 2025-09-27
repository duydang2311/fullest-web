import { env } from '$env/dynamic/private';
import { MemoryCache, type Cache } from '$lib/services/cache';
import { DefaultHttpClient } from '$lib/services/default_http_client';
import { withRuntime } from '$lib/utils/runtime';
import { redirect, type Handle, type ServerInit } from '@sveltejs/kit';
import { Cacheable } from 'cacheable';
import { jsonify } from './lib/utils/http';
import { withQueryParams } from './lib/utils/url';

let cache: Cache;

export const init: ServerInit = () => {
	if (!env.API_URL_PREFIX) {
		throw new Error('Expected API_URL_PREFIX env var');
	}
	if (!env.API_URL_SUFFIX) {
		throw new Error('Expected API_URL_SUFFIX env var');
	}
	if (!env.GOOGLE_OAUTH_CLIENT_ID) {
		throw new Error('Expected GOOGLE_OAUTH_CLIENT_ID env var');
	}
	if (!env.GOOGLE_OAUTH_CLIENT_SECRET) {
		throw new Error('Expected GOOGLE_OAUTH_CLIENT_SECRET env var');
	}
	cache = new MemoryCache(new Cacheable({ ttl: '30m' }));
};

export const handle: Handle = ({ event, resolve }) => {
	const routeId = event.route.id;
	if (routeId == null) {
		return resolve(event);
	}

	let sessionToken: string | null = null;
	let sessionRequired = false;
	if (routeId.includes('(auth)')) {
		sessionRequired = true;
		sessionToken = event.cookies.get('session_token') ?? null;
		if (!sessionToken) {
			return redirect(303, '/sign-in');
		}
	} else if (routeId.includes('(auth-optional)')) {
		sessionToken = event.cookies.get('session_token') ?? null;
	}

	event.locals = {
		http: new DefaultHttpClient({
			fetcher: globalThis.fetch,
			prefix: env.API_URL_PREFIX,
			suffix: env.API_URL_SUFFIX,
			headers: sessionToken ? { Authorization: `Bearer ${sessionToken}` } : undefined,
		}),
		cache,
	};

	if (sessionToken != null) {
		return event.locals.http
			.get(`/sessions/${sessionToken}`, {
				query: {
					fields: 'UserId',
					test: 'abc',
				},
			})
			.then(async (fetchedSession) => {
				if ((fetchedSession.failed || !fetchedSession.data.ok) && sessionRequired) {
					event.cookies.delete('session_token', {
						path: '/',
						httpOnly: true,
						secure: true,
						sameSite: 'lax',
					});
					return redirect(303, withQueryParams('/sign-in', { return_to: event.url.pathname }));
				}

				if (fetchedSession.ok) {
					const parsed = await jsonify(() => fetchedSession.data.json<{ userId: string }>());
					if (parsed.failed) {
						if (sessionRequired) {
							// TODO: handle parsed.error
							return redirect(303, '/sign-in');
						}
					} else {
						event.locals.session = parsed.data;
					}
				}
				return withRuntime(event.locals)(resolve, event);
			});
	}
	return withRuntime(event.locals)(resolve, event);
};
