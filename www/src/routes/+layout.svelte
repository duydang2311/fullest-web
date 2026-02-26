<script lang="ts">
    import '@fontsource-variable/dm-sans';
    import '../app.css';

    import favicon from '$lib/assets/favicon.svg';
    import { BrowserHttpClient } from '~/lib/services/browser_http_client';
    import { setRuntime } from '~/lib/utils/runtime';

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

{@render children?.()}
