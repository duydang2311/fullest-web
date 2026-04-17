<script lang="ts">
    import { SettingsOutline } from '~/lib/components/icons';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from './$types';

    const pageData = usePageData<PageData>();
</script>

<div>
    <div class="flex items-center gap-4 justify-between">
        <h2 class="text-fg-emph tracking-tight">Description</h2>
        <button type="button" class={C.button({ ghost: true, size: 'sm', icon: true })}>
            <SettingsOutline />
        </button>
    </div>
    {#if pageData.project.descriptionHtml}
        <div>
            {@html pageData.project.descriptionHtml}
        </div>
    {:else}
        <div class="text-fg-muted text-sm">Description not available.</div>
    {/if}
</div>
