<script lang="ts">
    import type { HTMLAttributes, HTMLOlAttributes } from 'svelte/elements';

    const {
        errors,
        transforms,
        class: cls,
        ...props
    }: {
        errors?: string[];
        transforms?: Record<string, string | ((requirement: any) => string)>;
    } & HTMLAttributes<HTMLElement> = $props();
</script>

{#snippet displayError(error: string)}
    {#if transforms}
        {@const [code, requirement] = error.split(':', 2)}
        {@const transform = transforms[code]}
        {#if typeof transform === 'function'}
            {transform(requirement)}
        {:else if typeof transform === 'string'}
            {transform}
        {:else}
            {error}
        {/if}
    {:else}
        {error}
    {/if}
{/snippet}

{#if errors != null}
    {#if errors.length > 1}
        <ol {...props} class="list-decimal pl-4.5 text-sm {cls}">
            {#each errors as error}
                <li class="pl-0">{@render displayError(error)}</li>
            {/each}
        </ol>
    {:else if errors.length === 1}
        <p {...props} class="text-sm {cls}">
            <span>{@render displayError(errors[0])}</span>
        </p>
    {/if}
{/if}
