import { DateTime } from 'luxon';

export function formatRelativeDateTime(iso: string | DateTime) {
    if (typeof iso === 'string') {
        iso = DateTime.fromISO(iso);
    }
    return iso.toRelative();
}
