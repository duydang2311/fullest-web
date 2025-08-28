import { env } from '$env/dynamic/private';
import { trimEnd, trimStart } from '$lib/utils/string';
import { type RequestHandler } from '@sveltejs/kit';

const slash = '/'.charCodeAt(0);
const prefix = trimEnd(env.API_URL_PREFIX, slash);
const suffix = trimStart(env.API_URL_SUFFIX, slash);
const start = '/api'.length;

export const fallback: RequestHandler = (e) => {
	const pathname = e.url.pathname.substring(start);
	const rewritten =
		pathname.length === 0
			? `${prefix}/${suffix}`
			: `${prefix}/${trimEnd(pathname, slash)}/${suffix}`;
	return e.locals.http.fetchRaw(rewritten, e.request);
};
