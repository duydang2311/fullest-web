<script lang="ts">
    import type { LayoutProps } from './$types';

    const { children }: LayoutProps = $props();
</script>

<div class="flex min-h-screen">
    <div class="flex-1 relative border-l border-l-surface-border">
        {@render children()}
    </div>
</div>
