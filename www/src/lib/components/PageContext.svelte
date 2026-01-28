<script lang="ts">
    import { page } from '$app/state';
    import { setContext, type Snippet } from 'svelte';
    import invariant from 'tiny-invariant';

    const { children }: { children: Snippet } = $props();

    invariant(page.route.id, 'page.route.id must not be null');
    setContext(
        page.route.id,
        new Proxy(
            {},
            {
                get(_, p) {
                    return page.data[p as keyof typeof page.data];
                },
            }
        )
    );
</script>

{@render children()}
