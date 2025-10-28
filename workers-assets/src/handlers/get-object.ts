import { getPath } from 'hono/utils/url';
import { handler, stripQueryParams, withR2Metadata } from '../lib/utils';

export const getObject = handler(async (req, env, ctx) => {
    const cache = caches.default;
    const cacheKey = stripQueryParams(req.url);
    let response = await cache.match(cacheKey);
    if (response) {
        return response;
    }

    const path = getPath(req);
    const obj = await env.BUCKET.get(path);
    if (obj == null) {
        return new Response(null, { status: 404 });
    }

    const headers = withR2Metadata(obj)(new Headers());
    headers.set('Cache-Control', 'public, max-age=3600, immutable');
    headers.set('ETag', obj.httpEtag);
    response = new Response(obj.body, { status: 200, headers });
    ctx.waitUntil(cache.put(cacheKey, response.clone()));
    return response;
});
