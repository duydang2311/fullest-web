<script lang="ts">
    import { page } from '$app/state';

    const snippets = {
        InvalidOAuthStateError: invalidOAuthState,
    };
    const snippet = $derived(
        page.error!.kind in snippets
            ? snippets[page.error!.kind as keyof typeof snippets]
            : defaultSnippet
    );
</script>

{#snippet invalidOAuthState()}
    <h1>Invalid OAuth state parameter</h1>
{/snippet}

{#snippet defaultSnippet()}
    <div class="flex flex-col justify-center items-center w-screen h-screen">
        <h1 class="text-fg-emph">Oops! Something went wrong</h1>
        {#if page.error}
            <div class="grid grid-cols-[auto_1fr] border rounded-sm border-base-border mt-4">
                {#each Object.entries(page.error) as [k, v] (k)}
                    <div class="col-span-full grid grid-cols-subgrid">
                        <div class="py-2 px-4 text-fg-muted">{k}</div>
                        <div class="py-2 px-4">{JSON.stringify(v)}</div>
                    </div>
                {/each}
            </div>
        {/if}
    </div>
{/snippet}

{@render snippet()}
