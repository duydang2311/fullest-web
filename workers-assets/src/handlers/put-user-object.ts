import { attempt } from '@duydang2311/attempt';
import { getPath } from 'hono/utils/url';
import { extractBearerToken, makeHeadersFrom, stripQueryParams, verifyJwt, withR2Metadata } from '../lib/utils';

export const putUserObject: ExportedHandlerFetchHandler<Env> = async (req, env, ctx) => {
    const verified = await extractBearerToken(req).pipe(
        attempt.flatMap((token) => verifyJwt(env.SIGNING_PUBLIC_KEY_PEM.replace(/\\n/g, '\n'))(token))
    );
    if (!verified.ok) {
        return new Response(null, { status: 401 });
    }

    const path = getPath(req);
    if (verified.data.object_key !== path || (await env.KV.get(verified.data.jti, 'text')) === '1') {
        return new Response(null, { status: 403 });
    }

    const version = Date.now();
    const obj = await env.BUCKET.put(path, await req.arrayBuffer(), {
        httpMetadata: {
            contentType: req.headers.get('Content-Type') ?? 'application/octet-stream',
        },
    });

    ctx.waitUntil(caches.default.delete(stripQueryParams(req.url)));
    ctx.waitUntil(env.KV.put(verified.data.jti, '1', { expirationTtl: 60 }));

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
};
