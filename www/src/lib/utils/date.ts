import { DateTime } from 'luxon';

export function formatRelativeDateTime(iso: string | DateTime) {
    if (typeof iso === 'string') {
        iso = DateTime.fromISO(iso);
    }
    const duration = iso.diffNow('days');
    if (duration.days < -3) {
        return formatDate(iso);
    }
    return iso.toRelative();
}

export function formatDate(iso: string | DateTime) {
    if (typeof iso === 'string') {
        iso = DateTime.fromISO(iso);
    }
    return iso.toLocaleString(DateTime.DATE_MED);
}
