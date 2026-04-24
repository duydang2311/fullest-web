import { env } from '$env/dynamic/private';
import { DefaultHttpClient } from '$lib/services/default_http_client';
import * as Sentry from '@sentry/sveltekit';
import { redirect, type Handle, type ServerInit } from '@sveltejs/kit';
import { sequence } from '@sveltejs/kit/hooks';
import { withRuntime } from '~/lib/utils/runtime.server';
import type { User, UserPreset } from './lib/models/user';
import { guardNull } from './lib/utils/guard';
import { jsonify } from './lib/utils/http';
import { trimEnd } from './lib/utils/string';
import { withQueryParams } from './lib/utils/url.server';

export const init: ServerInit = () => {
    guardNull(env.API_URL_PREFIX);
    guardNull(env.API_URL_SUFFIX);
    guardNull(env.GOOGLE_OAUTH_CLIENT_ID);
    guardNull(env.GOOGLE_OAUTH_CLIENT_SECRET);
};

const CODE_SLASH = '/'.charCodeAt(0);
const CODE_UNDERSCORE = '_'.charCodeAt(0);
const CODE_A = 'a'.charCodeAt(0);
const CODE_P = 'p'.charCodeAt(0);
const CODE_I = 'i'.charCodeAt(0);
export const handle: Handle = sequence(
    Sentry.initCloudflareSentryHandle({
        dsn: 'https://e6b2b167594545eaa399ac046d8c26fa@o1398941.ingest.us.sentry.io/4511273537110016',
        tracesSampleRate: 1.0,
        enableLogs: true,
        sendDefaultPii: true,
    }),
    Sentry.sentryHandle(),
    ({ event, resolve }) => {
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
            const searchIndex = event.request.url.indexOf('?');
            const search = searchIndex === -1 ? '' : event.request.url.substring(searchIndex);
            const headers = new Headers(event.request.headers);
            headers.delete('Cookie');
            headers.delete('Host');
            headers.delete('Origin');
            headers.delete('Referrer');
            if (!headers.has('Authorization')) {
                const token = event.cookies.get('session_token');
                if (token) {
                    headers.set('Authorization', `Bearer ${event.cookies.get('session_token')}`);
                }
            }
            return fetch(
                `${env.API_URL_PREFIX}/${trimEnd(pathname.substring(7), CODE_SLASH)}/${env.API_URL_SUFFIX}${search}`,
                new Request(event.request, { headers })
            );
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
            if (!event.locals.session && !sessionToken) {
                return redirect(303, '/sign-in');
            }
        } else if (routeId.includes('(auth-optional)')) {
            sessionToken = event.cookies.get('session_token') ?? null;
        }

        event.locals = {
            http: new DefaultHttpClient({
                fetcher: (url, init) => fetch(url, init),
                prefix: env.API_URL_PREFIX,
                suffix: env.API_URL_SUFFIX,
                headers: sessionToken ? { Authorization: `Bearer ${sessionToken}` } : undefined,
            }),
        };

        if (!event.locals.session && sessionToken != null) {
            return event.locals.http
                .get(`/sessions/${sessionToken}`, {
                    query: {
                        fields: 'User.Id,User.Name,User.DisplayName,User.ImageKey,User.ImageVersion',
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
                        fetchedSession.data.json<{
                            user: Pick<User, 'id'> & UserPreset['Avatar'];
                        }>()
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
    }
);

export const handleError = Sentry.handleErrorWithSentry();
