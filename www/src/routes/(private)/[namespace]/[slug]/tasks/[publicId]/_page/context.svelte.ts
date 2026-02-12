import { page } from '$app/state';

export function usePageData<T>() {
    return new Proxy(
        {},
        {
            get(_, p) {
                return page.data[p as keyof typeof page.data];
            },
        }
    ) as T;
}
