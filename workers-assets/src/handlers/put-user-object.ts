import { getPath } from 'hono/utils/url';
import { privateHandler, stripQueryParams, withR2Metadata } from '../lib/utils';

export const putUserObject = privateHandler(async (req, env, ctx) => {
    const path = getPath(req);
    const version = Date.now();
    const obj = await env.BUCKET.put(path, await req.arrayBuffer(), {
        httpMetadata: {
            contentType: req.headers.get('Content-Type') ?? 'application/octet-stream',
        },
    });

    ctx.waitUntil(caches.default.delete(stripQueryParams(req.url)));
    ctx.waitUntil(env.KV.put(env.JWT.jti, '1', { expirationTtl: 60 }));

    return Response.json(
        {
            key: obj.key,
            version,
            size: obj.size,
            uploaded: obj.uploaded,
        },
        {
            status: 200,
            headers: withR2Metadata(obj)(new Headers()),
        }
    );
});
