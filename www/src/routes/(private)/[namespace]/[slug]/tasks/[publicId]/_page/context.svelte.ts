import { page } from '$app/state';
import { getContext } from 'svelte';

export function usePageData<T>() {
    return getContext<T>(page.route.id);
}
