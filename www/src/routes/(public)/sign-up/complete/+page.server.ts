import { CacheKey } from '$lib/utils/cache';
import { GenericError, ValidationError } from '$lib/utils/errors';
import { jsonify, parseHttpProblem } from '$lib/utils/http';
import { createValidator } from '$lib/utils/validation';
import { error, fail, redirect } from '@sveltejs/kit';
import * as v from 'valibot';
import type { Actions } from './$types';
import invariant from 'tiny-invariant';

export const actions: Actions = {
    default: async (e) => {
        const id = e.cookies.get('oauth_complete_session');
        if (!id) {
            return fail(400, ValidationError({ $: ['ERR_NO_COMPLETE_SESSION_COOKIE'] }));
        }

        const formData = await e.request.formData();
        const formParsed = validator.parse(decode(formData));
        if (!formParsed.ok) {
            return fail(400, formParsed.error);
        }

        const key = CacheKey.completeOAuthRegistration(id);
        invariant(e.platform, 'e.platform must not be null');
        const dataStr = await e.platform.env.APP_KV.get(key);
        if (!dataStr) {
            return fail(400, ValidationError({ $: ['ERR_NO_COMPLETE_SESSION_DATA'] }));
        }

        await e.platform.env.APP_KV.delete(key);

        const result = completeSessionDataValidator.parse(JSON.stringify(dataStr));
        if (!result.ok) {
            return fail(400, result.error);
        }

        switch (result.data.provider) {
            case 'google': {
                const created = await e.locals.http.post('users', {
                    body: {
                        name: formParsed.data.name,
                        provider: 'google',
                        googleIdToken: result.data.idToken,
                    },
                });
                if (!created.ok) {
                    return error(500, created.error);
                }
                if (!created.data.ok) {
                    const problemParsed = parseHttpProblem(created.data);
                    if (!problemParsed.ok) {
                        return error(500, problemParsed.error);
                    }
                    return fail(created.data.status, ValidationError.from(problemParsed.data));
                }

                const sessionCreated = await e.locals.http.post('sessions', {
                    body: {
                        provider: 'google',
                        googleIdToken: result.data.idToken,
                    },
                });
                if (!sessionCreated.ok) {
                    return error(500, sessionCreated.error);
                }
                if (!sessionCreated.data.ok) {
                    const problemParsed = await parseHttpProblem(sessionCreated.data);
                    if (!problemParsed.ok) {
                        return error(500, problemParsed.error);
                    }
                    return fail(
                        sessionCreated.data.status,
                        ValidationError.from(problemParsed.data)
                    );
                }

                const jsonified = await jsonify(() =>
                    sessionCreated.data.json<{
                        token: string;
                    }>()
                );
                if (!jsonified.ok) {
                    return error(500, jsonified.error);
                }

                e.cookies.set('session_token', jsonified.data.token, {
                    path: '/',
                    maxAge: 60 * 60 * 24 * 7,
                    httpOnly: true,
                    secure: true,
                    sameSite: 'lax',
                });
                return redirect(303, '/');
            }
            default:
                return error(500, GenericError({ code: 'ERR_INVALID_OAUTH_PROVIDER' }));
        }
    },
};

const decode = (formData: FormData) => {
    return {
        name: formData.get('name'),
    };
};

const validator = createValidator(
    v.object({
        name: v.pipe(v.string(), v.regex(/^[a-zA-Z0-9_\\-]{3,}$/)),
    })
);

const completeSessionDataValidator = createValidator(
    v.variant('provider', [
        v.object({
            provider: v.literal('google'),
            idToken: v.string(),
        }),
    ])
);
