<script lang="ts">
    import type { Snippet } from 'svelte';

    interface Props {
        header: Snippet;
        body: Snippet;
    }

    const { header, body }: Props = $props();
</script>

<div class="border border-base-border rounded-lg">
    <div class="border-b border-base-border px-4 py-2 bg-surface-subtle rounded-t-lg">
        {@render header()}
    </div>
    <div class="p-4">
        {@render body()}
    </div>
</div>
