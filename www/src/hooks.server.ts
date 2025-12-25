import { env } from '$env/dynamic/private';
import { MemoryCache, type Cache } from '$lib/services/cache';
import { DefaultHttpClient } from '$lib/services/default_http_client';
import { withRuntime } from '$lib/utils/runtime';
import { redirect, type Handle, type ServerInit } from '@sveltejs/kit';
import { Cacheable } from 'cacheable';
import invariant from 'tiny-invariant';
import { jsonify } from './lib/utils/http';
import { trimEnd } from './lib/utils/string';
import { withQueryParams } from './lib/utils/url';

let cache: Cache;

export const init: ServerInit = () => {
    invariant(env.API_URL_PREFIX, 'expect API_URL_PREFIX');
    invariant(env.API_URL_SUFFIX, 'expect API_URL_SUFFIX');
    invariant(env.GOOGLE_OAUTH_CLIENT_ID, 'expect GOOGLE_OAUTH_CLIENT_ID');
    invariant(env.GOOGLE_OAUTH_CLIENT_SECRET, 'expect GOOGLE_OAUTH_CLIENT_SECRET');
    cache = new MemoryCache(new Cacheable({ ttl: '30m' }));
};

const CODE_SLASH = '/'.charCodeAt(0);
const CODE_UNDERSCORE = '_'.charCodeAt(0);
const CODE_A = 'a'.charCodeAt(0);
const CODE_P = 'p'.charCodeAt(0);
const CODE_I = 'i'.charCodeAt(0);
export const handle: Handle = ({ event, resolve }) => {
    const pathname = event.url.pathname;
    if (
        pathname.charCodeAt(0) === CODE_SLASH &&
        pathname.charCodeAt(1) === CODE_UNDERSCORE &&
        pathname.charCodeAt(2) === CODE_SLASH &&
        pathname.charCodeAt(3) === CODE_A &&
        pathname.charCodeAt(4) === CODE_P &&
        pathname.charCodeAt(5) === CODE_I &&
        pathname.charCodeAt(6) === CODE_SLASH
    ) {
        const request = new Request(
            `${env.API_URL_PREFIX}/${trimEnd(pathname.substring(7), CODE_SLASH)}/${env.API_URL_SUFFIX}`,
            event.request
        );
        if (!request.headers.has('Authorization')) {
            const token = event.cookies.get('session_token');
            if (token) {
                request.headers.set(
                    'Authorization',
                    `Bearer ${event.cookies.get('session_token')}`
                );
            }
        }
        return fetch(request);
    }

    const routeId = event.route.id;
    if (routeId == null) {
        return resolve(event);
    }

    let sessionToken: string | null = null;
    let isPrivateRoute = false;
    if (routeId.includes('(private)')) {
        isPrivateRoute = true;
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
                    fields: 'User.Id,User.Name',
                    test: 'abc',
                },
            })
            .then(async (fetchedSession) => {
                if (!fetchedSession.ok) {
                    if (isPrivateRoute) {
                        return redirect(
                            303,
                            withQueryParams('/sign-in', { return_to: event.url.pathname })
                        );
                    }
                    return withRuntime(event.locals)(resolve, event);
                }

                if (!fetchedSession.data.ok) {
                    event.cookies.delete('session_token', {
                        path: '/',
                        httpOnly: true,
                        secure: true,
                        sameSite: 'lax',
                    });
                    if (isPrivateRoute) {
                        return redirect(
                            303,
                            withQueryParams('/sign-in', { return_to: event.url.pathname })
                        );
                    }
                }

                const parsed = await jsonify(() =>
                    fetchedSession.data.json<{ user: { id: string; name: string } }>()
                );
                if (!parsed.ok) {
                    if (isPrivateRoute) {
                        // TODO: handle parsed.error
                        return redirect(303, '/sign-in');
                    }
                } else {
                    event.locals.session = parsed.data;
                }
                return withRuntime(event.locals)(resolve, event);
            });
    }
    return withRuntime(event.locals)(resolve, event);
};
