<script lang="ts">
    import { portal } from '@zag-js/svelte';
    import { createTooltip } from './builders.svelte';

    const { content }: { content: string } = $props();
    const id = $props.id();
    const tooltip = createTooltip({
        id,
        openDelay: 400,
        closeDelay: 0,
    });

    export function getTooltip() {
        return tooltip;
    }
</script>

<div>
    {#if tooltip.api.open}
        <div use:portal {...tooltip.api.getPositionerProps()}>
            <div
                {...tooltip.api.getArrowProps()}
                style:--arrow-size="0.5rem"
                style:--arrow-background="var(--color-fg-emph)"
            >
                <div {...tooltip.api.getArrowTipProps()}></div>
            </div>
            <div
                {...tooltip.api.getContentProps()}
                class="bg-fg-emph text-surface rounded-md px-2 py-1 text-sm font-medium"
            >
                {content}
            </div>
        </div>
    {/if}
</div>
