<script lang="ts">
    import { page } from '$app/state';
    import { untrack } from 'svelte';
    import { PlusOutline } from '~/lib/components/icons';
    import { keysetList } from '~/lib/models/paginated';
    import { StatusCategory } from '~/lib/models/status';
    import { button } from '~/lib/utils/styles';
    import StatusGroup from './__page__/StatusGroup.svelte';
    import { setPageContext } from './__page__/utils.svelte';

    const { data } = $props();
    const ctx = setPageContext({ taskGroups: untrack(() => data.taskList) });
</script>

<div class="mx-auto max-w-container-lg w-full">
    <div class="flex gap-4 items-start justify-between">
        <div>
            <h1 class="text-fg-emph text-title-xs">Tasks</h1>
        </div>
        <a
            href="/{page.params.namespace}/{page.params.slug}/tasks/new"
            class="{button({
                filled: true,
                size: 'sm',
            })} ml-auto flex items-center gap-2 text-nowrap"
        >
            <PlusOutline />
            New task
        </a>
    </div>
    <div class="mt-4">
        <div class="grid grid-cols-[auto_auto_auto_1fr_auto_auto]">
            {#if ctx.taskGroups['none']}
                {@const list = ctx.taskGroups['none'] ?? keysetList()}
                <StatusGroup
                    status={{
                        id: 'none',
                        name: 'None',
                        color: 'var(--color-status-none)',
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
