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
    import { useActivityLists, usePageContext, usePriorityList, useTask } from './utils.svelte';
    import { getPriorityIcon } from '~/lib/utils/priority';

    const data = usePageData<PageData>();
    const ctx = usePageContext();
    const activityLists = $derived(useActivityLists(ctx.activityListParams));
    const task = $derived(await useTask());
    const id = $props.id();
    let priorityList = $state.raw<Awaited<ReturnType<typeof usePriorityList>>>();
    const menu = createMenu({
        id,
        defaultHighlightedValue: untrack(() => task.priority?.id),
        async onOpenChange(details) {
            if (details.open) {
                priorityList ??= await usePriorityList().run();
                menu.api.setHighlightedValue(task.priority?.id);
            }
        },
        onSelect(details) {
            return updatePriority(details.value);
        },
    });

    async function updatePriority(priorityId: string) {
        guardNull(priorityList);
        const priority = priorityList.items.find((a) => a.id === priorityId);
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
            {#each priorityList?.items as priority (priority.id)}
                {@const Icon = getPriorityIcon(priority)}
                <li>
                    <button
                        {...menu.api.getItemProps({ value: priority.id })}
                        type="button"
                        class="{C.menu({ part: 'item' })} flex items-center gap-2 w-full"
                    >
                        <Icon />
                        <span>{priority.name}</span>
                    </button>
                </li>
            {/each}
        </ul>
    </div>
</div>
