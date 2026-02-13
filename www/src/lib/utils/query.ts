import { browser } from '$app/environment';
import { QueryClient } from '@tanstack/svelte-query';

export const QueryKey = {
    TaskDetailsPage: {
        ofTask: (args: { taskId: string }) => ['tasks', args] as const,
        ofActivityList: (args: { taskId: string }) => ['activities', args] as const,
    },
};

let browserClient: QueryClient | null = null;
const serverClient = new QueryClient({
    defaultOptions: {
        queries: {
            enabled: false,
        },
    },
});
export function useQueryClient() {
    if (browser) {
        browserClient ??= new QueryClient({
            defaultOptions: {
                queries: {
                    enabled: browser,
                },
            },
        });
        return browserClient;
    }
    return serverClient;
}
