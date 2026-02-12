<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import { portal } from '@zag-js/svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { SettingsOutline } from '~/lib/components/icons';
    import type { Status } from '~/lib/models/status';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { usePageData } from './context.svelte';
    import { getPriorities, patchTaskPriority } from './page.remote';

    const data = usePageData<PageData>();
    const id = $props.id();
    const menu = createMenu({
        id,
        onOpenChange: async (details) => {
            if (details.open) {
                menu.api.setHighlightedValue(data.task.priority?.id ?? '');
                statuses = await getPriorities(data.project.id).then((a) => a.items);
            }
        },
        onSelect: async (details) => {
            await patchTaskPriority({
                taskId: data.task.id,
                priorityId: details.value,
            }).then(invalidateAll);
        },
    });

    let statuses = $state.raw<Pick<Status, 'id' | 'name'>[]>();
</script>

<div class="text-sm">
    <button
        {...menu.api.getTriggerProps()}
        type="button"
        class="{C.button({
            variant: 'base',
            ghost: true,
        })} text-left font-medium w-full flex items-center max-lg:flex-row-reverse max-lg:justify-end gap-2 lg:justify-between"
    >
        <span>
            {data.task.priority?.name ?? 'No priority'}
        </span>
        <SettingsOutline />
    </button>
    <div use:portal {...menu.api.getPositionerProps()}>
        <ul
            {...menu.api.getContentProps()}
            class="{C.menu({ part: 'content' })} flex flex-col gap-1 w-(--reference-width)"
        >
            {#each statuses as priority (priority.id)}
                <li>
                    <button
                        {...menu.api.getItemProps({ value: priority.id })}
                        type="button"
                        class="{C.menu({ part: 'item' })} flex items-center gap-4 w-full"
                    >
                        {priority.name}
                    </button>
                </li>
            {/each}
        </ul>
    </div>
</div>
