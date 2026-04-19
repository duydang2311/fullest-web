<script lang="ts">
    import { page } from '$app/state';
    import { untrack } from 'svelte';
    import { PlusOutline } from '~/lib/components/icons';
    import { keysetList } from '~/lib/models/paginated';
    import { StatusCategory } from '~/lib/models/status';
    import { button } from '~/lib/utils/styles';
    import Breadcrumbs from '../__page__/Breadcrumbs.svelte';
    import StatusGroup from './StatusGroup.svelte';
    import { setPageContext } from './utils.svelte';

    const { data } = $props();

    const ctx = setPageContext({ taskGroups: untrack(() => data.taskList) });
</script>

<div class="py-4">
    <div class="px-8 pb-4 border-b border-b-surface-border">
        <div class="flex gap-4 items-start justify-between">
            <div>
                <Breadcrumbs />
                <h1 class="tracking-tight text-fg-emph mt-1 text-title-xs">Tasks</h1>
            </div>
            <a
                href="/{page.params.namespace}/{page.params.slug}/tasks/new"
                class="{button({
                    variant: 'primary',
                    filled: true,
                    size: 'sm',
                })} ml-auto flex items-center gap-2 text-nowrap tracking-tight"
            >
                <PlusOutline />
                New
            </a>
        </div>
    </div>
    <div class="mt-4 px-4">
        <div class="grid grid-cols-[auto_1fr_auto_auto] border border-surface-border rounded-md">
            {#if ctx.taskGroups['none']}
                {@const list = ctx.taskGroups['none'] ?? keysetList()}
                <StatusGroup
                    status={{
                        id: 'none',
                        name: 'None',
                        color: '',
                        category: StatusCategory.Proposed,
                    }}
                    {list}
                />
            {/if}
            {#each data.statusList.items as item (item.id)}
                {@const list = ctx.taskGroups[item.id] ?? keysetList()}
                <StatusGroup status={item} {list} />
            {/each}
        </div>
    </div>
</div>
