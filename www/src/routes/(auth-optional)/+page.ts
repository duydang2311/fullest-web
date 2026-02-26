import type { PageLoad } from './$types';

export const load: PageLoad = async (e) => {
    if (e.data.session) {
        return {
            ...e.data,
            Authenticated: await import('./Authenticated.svelte').then((a) => a.default),
        };
    }
    return e.data;
};
