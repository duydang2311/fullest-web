<script lang="ts">
    import { page } from '$app/state';
    import { untrack } from 'svelte';
    import { PlusOutline } from '~/lib/components/icons';
    import { keysetList } from '~/lib/models/paginated';
    import { StatusCategory } from '~/lib/models/status';
    import { button } from '~/lib/utils/styles';
    import ContentLayout from '../ContentLayout.svelte';
    import StatusGroup from './StatusGroup.svelte';
    import { setPageContext } from './utils.svelte';

    const { data } = $props();

    const ctx = setPageContext({ taskGroups: untrack(() => data.taskList) });
</script>

<ContentLayout>
    <div class="flex gap-4 border-b border-b-surface-border pb-4 px-8 items-center justify-between">
        <h1 class="tracking-tight text-fg-emph">Tasks</h1>
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
    <div class="mt-4 px-4">
        <div
            class="grid grid-cols-[auto_1fr_auto_auto] border border-surface-border rounded-md gap-px"
        >
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
</ContentLayout>
