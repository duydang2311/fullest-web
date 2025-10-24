import { PatternRouter } from 'hono/router/pattern-router';
import { getPath } from 'hono/utils/url';
import { getObject } from './handlers/get-object';
import { putUserObject } from './handlers/put-user-object';
import { isAllowedOrigin, withCors } from './lib/utils';

const router = new PatternRouter<ExportedHandlerFetchHandler<Env>>();

router.add('GET', '/users/:userId/*', getObject);
router.add('PUT', '/users/:userId/*', putUserObject);

export default {
    async fetch(req, env, ctx) {
        const requestOrigin = req.headers.get('Origin');
        if (requestOrigin == null || !isAllowedOrigin(env.ALLOWED_ORIGINS)(requestOrigin)) {
            return new Response(null, { status: 403 });
        }

        const requestHeaders = req.headers.get('Access-Control-Request-Headers') ?? '*';
        if (req.method === 'OPTIONS') {
            return new Response(null, {
                status: 204,
                headers: withCors(requestOrigin, requestHeaders)(new Headers()),
            });
        }
        const result = router.match(req.method, getPath(req))[0];
        // TODO: middleware support?
        let response: Response | null = null;
        if (result.length > 0) {
			// TODO: catch exception and return with cors headers
            response = await result[0][0](req, env, ctx);
        } else {
            response = new Response(null, { status: 404 });
        }
        withCors(requestOrigin, requestHeaders)(response.headers);
        return response;
    },
} satisfies ExportedHandler<Env>;
