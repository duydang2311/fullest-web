import { attempt } from '@duydang2311/attempt';
import { getPath } from 'hono/utils/url';
import { importSPKI, jwtVerify } from 'jose';

export function stripQueryParams(url: string) {
    const questionMark = url.indexOf('?');
    return questionMark === -1 ? url : url.substring(0, questionMark);
}

export function extractBearerToken(request: Request) {
    const header = request.headers.get('Authorization');
    if (header == null) {
        return attempt.fail('MISSING_HEADER' as const);
    }
    if (!header.startsWith('Bearer')) {
        return attempt.fail('INVALID_SCHEME' as const);
    }
    const start = 7; // 'Bearer '.length
    return attempt.ok(header.substring(start));
}

export function verifyJwt(publicKeyPem: string) {
    const imported = importSPKI(publicKeyPem, 'RS256');
    return async (token: string) => {
        const publicKey = await imported;
        const verified = await jwtVerify(token, publicKey, {
            issuer: 'api',
            audience: 'workers-assets',
            requiredClaims: ['sub', 'jti', 'object_key', 'mime_type'],
            clockTolerance: '5 seconds',
            maxTokenAge: '1 minute',
        });
        if (verified.payload.jti == null) {
            return attempt.fail('MISSING_JTI' as const);
        }
        if (verified.payload.sub == null) {
            return attempt.fail('MISSING_SUB' as const);
        }
        if (verified.payload.object_key == null) {
            return attempt.fail('MISSING_OBJECT_KEY' as const);
        }
        if (verified.payload.mime_type == null) {
            return attempt.fail('MISSING_MIME_TYPE' as const);
        }
        return attempt.ok(
            verified.payload as {
                jti: string;
                sub: string;
                object_key: string;
                mime_type: string;
            }
        );
    };
}

export function makeHeadersFrom(obj: R2Object) {
    const headers = new Headers();
    obj.writeHttpMetadata(headers);
    return headers;
}

export function withR2Metadata(obj: R2Object) {
    return (headers: Headers) => {
        obj.writeHttpMetadata(headers);
        return headers;
    };
}

export function withCors(allowedOrigin: string, allowedHeaders: string) {
    return (headers: Headers) => {
        headers.set('Access-Control-Allow-Origin', allowedOrigin);
        headers.set('Access-Control-Allow-Methods', 'OPTIONS,GET,PUT');
        headers.set('Access-Control-Allow-Headers', allowedHeaders);
        headers.set('Access-Control-Max-Age', '86400');
        headers.set('Access-Control-Allow-Credentials', 'true');
        if (allowedOrigin !== '*' && !headers.has('Vary')) {
            headers.set('Vary', allowedOrigin);
        }
        return headers;
    };
}

export function isAllowedOrigin(allowedOrigins: string[]) {
    return (origin: string) => {
        return allowedOrigins.includes(origin);
    };
}

export function handler(f: ExportedHandlerFetchHandler<Env>) {
    return f;
}

type AuthenticatedEnv = Omit<Env, 'JWT'> & { JWT: NonNullable<Env['JWT']> };
export function privateHandler(f: ExportedHandlerFetchHandler<AuthenticatedEnv>) {
    return handler(async (req, env, ctx) => {
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

        env.JWT = verified.data;
        return f(req, env as AuthenticatedEnv, ctx);
    });
}
