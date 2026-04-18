<script lang="ts">
    import { portal } from '@zag-js/svelte';
    import { untrack } from 'svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { SettingsOutline } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import type { Priority } from '~/lib/models/priority';
    import { guardNull } from '~/lib/utils/guard';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { getActivityList, getPriorities, patchTaskPriority } from './page.remote';
    import { useActivityLists, usePageContext, useTask } from './utils.svelte';

    const data = usePageData<PageData>();
    const task = $derived(await useTask());
    const ctx = usePageContext();
    const activityLists = $derived(useActivityLists(ctx.activityListParams));
    const id = $props.id();
    const menu = createMenu({
        id,
        defaultHighlightedValue: untrack(() => task.priority?.id),
        onOpenChange: async (details) => {
            if (details.open) {
                priorities = await getPriorities(data.project.id)
                    .run()
                    .then((a) => a.items);
                menu.api.setHighlightedValue(task.priority?.id);
            }
        },
        onSelect: (details) => updatePriority(details.value),
    });

    async function updatePriority(priorityId: string) {
        const priority = priorities?.find((a) => a.id === priorityId);
        guardNull(priority);
        const lastList = activityLists.at(-1);
        guardNull(lastList);
        guardNull(lastList.query.current);
        const oldPriority = task.priority;
        const optimisticActivity = {
            id: self.crypto.randomUUID(),
            actor: data.user,
            createdTime: new Date().toISOString(),
            kind: ActivityKind.PriorityChanged,
            metadata: {
                priority,
                oldPriority,
            },
        };
        await patchTaskPriority({
            taskId: task.id,
            priorityId,
            version: task.version,
        }).updates(
            useTask().withOverride((task) => ({
                ...task,
                priority,
            })),
            getActivityList(lastList.param).withOverride((list) => {
                return {
                    ...list,
                    items: [...list.items, optimisticActivity],
                };
            })
        );
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
            {task.priority?.name ?? 'No priority'}
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
