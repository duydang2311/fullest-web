<script lang="ts">
    import type { Snippet } from 'svelte';
    import type { HTMLAttributes } from 'svelte/elements';

    interface Props extends HTMLAttributes<HTMLDivElement> {
        header: Snippet;
        body: Snippet;
    }

    const { header, body, ...props }: Props = $props();
</script>

<div {...props}>
    <div class="border border-b-0 border-base-border px-4 py-2 bg-surface-subtle rounded-t-xl">
        {@render header()}
    </div>
    <div class="p-4 border border-base-border bg-surface rounded-b-xl">
        {@render body()}
    </div>
</div>
