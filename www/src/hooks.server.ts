import { env } from '$env/dynamic/private';
import { MemoryCache, type Cache } from '$lib/services/cache';
import { DefaultHttpClient } from '$lib/services/default_http_client';
import { withRuntime } from '$lib/utils/runtime';
import type { Handle, ServerInit } from '@sveltejs/kit';
import { Cacheable } from 'cacheable';

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

	console.log('init called');
	cache = new MemoryCache(new Cacheable({ ttl: '1h' }));
};

export const handle: Handle = ({ event, resolve }) => {
	const session = event.cookies.get('session');
	const headers = session ? { Authorization: `Bearer ${session}` } : undefined;
	event.locals = {
		http: new DefaultHttpClient({
			fetcher: globalThis.fetch,
			prefix: env.API_URL_PREFIX,
			suffix: env.API_URL_SUFFIX,
			headers,
		}),
		cache,
	};
	return withRuntime(event.locals)(resolve, event);
};
