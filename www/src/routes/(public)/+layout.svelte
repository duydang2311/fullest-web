<script lang="ts">
    import FallingPattern from '~/lib/components/FallingPattern.svelte';
    import type { LayoutProps } from './$types';

    const { children }: LayoutProps = $props();
</script>

<div class="flex min-h-screen">
    <FallingPattern class="h-screen sticky top-0 flex-4" />
    <div class="flex-6 border-l border-l-surface-border">
        {@render children()}
    </div>
</div>
