<script lang="ts">
    import type { Snippet } from 'svelte';
    import type { HTMLAttributes } from 'svelte/elements';

    interface Props extends HTMLAttributes<HTMLDivElement> {
        header: Snippet;
        body: Snippet;
    }

    const { header, body, class: cls, ...props }: Props = $props();
</script>

<div {...props} class="border border-base-border rounded-lg {cls}">
    <div class="border-b border-base-border px-4 py-2 bg-surface-subtle rounded-t-lg">
        {@render header()}
    </div>
    <div class="p-4">
        {@render body()}
    </div>
</div>
