<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import { portal } from '@zag-js/svelte';
    import invariant from 'tiny-invariant';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { SettingsOutline } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import type { Priority } from '~/lib/models/priority';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { getPriorities, patchTaskPriority } from './page.remote';
    import { usePageContext } from './utils.svelte';

    const data = usePageData<PageData>();
    const ctx = usePageContext();
    const id = $props.id();
    const menu = createMenu({
        id,
        onOpenChange: async (details) => {
            if (details.open) {
                menu.api.setHighlightedValue(ctx.task.priority?.id ?? '');
                priorities = await getPriorities(data.project.id).run().then((a) => a.items);
            }
        },
        onSelect: (details) => updatePriority(details.value),
    });

    async function updatePriority(priorityId: string) {
        const oldTask = $state.snapshot(ctx.task);
        const oldActivityList = $state.snapshot(ctx.activityList);
        invariant(oldTask, 'oldTask must not be null');
        invariant(oldActivityList, 'oldActivityList must not be null');

        const priority = priorities?.find((a) => a.id === priorityId);
        invariant(priority, 'priority must not be null');

        ctx.activityList = {
            ...oldActivityList,
            items: [
                ...oldActivityList.items,
                {
                    id: self.crypto.randomUUID(),
                    actor: data.user,
                    createdTime: new Date().toISOString(),
                    kind: ActivityKind.PriorityChanged,
                    metadata: {
                        priority: {
                            id: priorityId,
                            name: priority.name,
                        },
                        oldPriority: {
                            id: oldTask.priority?.id,
                            name: oldTask.priority?.name ?? 'No priority',
                        },
                    },
                },
            ],
        };
        ctx.task = {
            ...oldTask,
            priority,
        };
        await patchTaskPriority({
            taskId: ctx.task.id,
            priorityId,
        }).finally(invalidateAll);
    }

    let priorities = $state.raw<Pick<Priority, 'id' | 'name'>[]>();
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
            {ctx.task.priority?.name ?? 'No priority'}
        </span>
        <SettingsOutline />
    </button>
    <div use:portal {...menu.api.getPositionerProps()}>
        <ul
            {...menu.api.getContentProps()}
            class="{C.menu({ part: 'content' })} flex flex-col gap-1 w-(--reference-width)"
        >
            {#each priorities as priority (priority.id)}
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
