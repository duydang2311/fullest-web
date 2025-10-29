import { PatternRouter } from 'hono/router/pattern-router';
import { getPath } from 'hono/utils/url';
import { getObject } from './handlers/get-object';
import { putUserObject } from './handlers/put-user-object';
import { putUserPfp } from './handlers/put-user-pfp';
import { setCors } from './lib/utils';

declare global {
    interface Env extends Cloudflare.Env {
        JWT?: { jti: string; sub: string };
    }
}

const router = new PatternRouter<ExportedHandlerFetchHandler<Env>>();

router.add('PUT', '/users/:userId/pfp', putUserPfp);
router.add('GET', '/users/:userId/*', getObject);
router.add('PUT', '/users/:userId/*', putUserObject);

export default {
    async fetch(req, env, ctx) {
        const origin = req.headers.get('Origin') || '*';
        const headers = req.headers.get('Access-Control-Request-Headers') ?? '*';
        if (req.method === 'OPTIONS') {
            return new Response(null, {
                status: 204,
                headers: setCors(origin, headers)(new Headers()),
            });
        }

        const path = getPath(req);
        const result = router.match(req.method, path)[0];
        let response: Response | null = null;
        if (result.length > 0) {
            const handler = result[0][0];
            // TODO: catch exception and return with cors headers
            response = await handler(req, env, ctx);
        } else {
            response = new Response(null, { status: 404 });
        }
        setCors(origin, headers)(response.headers);
        return response;
    },
} satisfies ExportedHandler<Env>;
