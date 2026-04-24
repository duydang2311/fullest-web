import { isAttempt } from '@duydang2311/attempt';
import * as Sentry from '@sentry/sveltekit';
import { isHttpError, type ServerLoad } from '@sveltejs/kit';
import { isPromise } from 'is-what';
import { isHttpErr, isHttpValidationErr } from './errors';

export function withObservability<F extends (...args: any[]) => Promise<unknown>>(
    func: F,
    onCaptured?: (...args: Parameters<F>) => void
): F;
export function withObservability<F extends (...args: any[]) => unknown>(
    func: F,
    onCaptured?: (...args: Parameters<F>) => void
): F;
export function withObservability<F extends (...args: any[]) => unknown>(
    func: F,
    onCaptured?: (...args: Parameters<F>) => void
): (...args: Parameters<F>) => Awaited<ReturnType<F>> | Promise<Awaited<ReturnType<F>>> {
    return (...args) => {
        try {
            const value = func(...args);
            if (isPromise(value)) {
                return value
                    .then((value) => {
                        if (isAttempt(value) && !value.ok) {
                            captureError(value.error, onCaptured, args);
                        }
                        return value as Awaited<ReturnType<F>>;
                    })
                    .catch((e) => {
                        captureException(e, onCaptured, args);
                        throw e;
                    });
            }
            if (isAttempt(value) && !value.ok) {
                captureError(value.error, onCaptured, args);
            }
            return value as Awaited<ReturnType<F>>;
        } catch (e) {
            captureException(e, onCaptured, args);
            throw e;
        }
    };
}

function captureError(err: unknown, cb: Function | undefined | null, args: any[]) {
    if ((isHttpErr(err) || isHttpValidationErr(err)) && err.status >= 500) {
        const traceId = Sentry.getActiveSpan()?.spanContext().traceId;
        if (traceId) {
            (err as any).traceId = traceId;
        }
        (err as any).eventId = Sentry.captureException(err, {
            mechanism: { type: 'generic.observability.capture_error', handled: true },
        });
        cb?.(...args);
    }
}

function captureException(ex: unknown, cb: Function | undefined | null, args: any[]) {
    if (isHttpError(ex) && ex.status >= 500) {
        const traceId = Sentry.getActiveSpan()?.spanContext().traceId;
        (ex as any).message = ex.body.message;
        if (traceId) {
            (ex as any).body.traceId = traceId;
        }
        (ex as any).body.eventId = Sentry.captureException(ex, {
            mechanism: {
                type: 'generic.observability.capture_exception',
                handled: true,
            },
        });
        cb?.(...args);
    }
}

export function observableServerLoad<F extends ServerLoad<any, any, any, any>>(func: F) {
    return withObservability(func, async (e) => {
        if (e.platform?.ctx.waitUntil) {
            e.platform.ctx.waitUntil(
                new Promise((resolve) => {
                    Sentry.flush(2000).then(resolve);
                })
            );
        } else {
            await Sentry.flush(2000);
        }
    });
}
