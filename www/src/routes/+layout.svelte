<script lang="ts">
    import '@fontsource-variable/geist-mono';
    import '@fontsource-variable/inter';
    import '../app.css';

    import favicon from '$lib/assets/favicon.svg';
    import { QueryClientProvider } from '@tanstack/svelte-query';
    import { useQueryClient } from '~/lib/utils/query';
    import { setRuntime } from '~/lib/utils/runtime';
    import { BrowserHttpClient } from '~/lib/services/browser_http_client';

    const { children } = $props();

    setRuntime({
        http: new BrowserHttpClient({
            fetcher: fetch,
            prefix: '/_/api/',
        }),
    });
</script>

<svelte:head>
    <link rel="icon" href={favicon} />
</svelte:head>

<QueryClientProvider client={useQueryClient()}>
    {@render children?.()}
</QueryClientProvider>
