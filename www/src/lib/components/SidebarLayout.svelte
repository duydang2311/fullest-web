<script lang="ts">
    import type { Snippet } from 'svelte';

    const { top, left, main }: { top: Snippet; left: Snippet; main: Snippet } = $props();
</script>

<div class="bg-surface-subtle flex flex-col h-screen overflow-hidden">
    {@render top()}
    <div class="flex flex-1 overflow-hidden">
        <div class="min-w-72">
            {@render left()}
        </div>
        <div
            class="flex-1 bg-surface border-t border-l border-surface-border rounded-tl-2xl flex flex-col overflow-auto"
        >
            {@render main()}
        </div>
    </div>
</div>
