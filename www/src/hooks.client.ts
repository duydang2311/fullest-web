import { handleErrorWithSentry, replayIntegration } from '@sentry/sveltekit';
import * as Sentry from '@sentry/sveltekit';

Sentry.init({
    dsn: 'https://e6b2b167594545eaa399ac046d8c26fa@o1398941.ingest.us.sentry.io/4511273537110016',
    tracesSampleRate: 1.0,
    enableLogs: true,
    sendDefaultPii: true,
});

export const handleError = handleErrorWithSentry();
