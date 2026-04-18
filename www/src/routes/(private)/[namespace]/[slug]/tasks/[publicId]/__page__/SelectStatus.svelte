<script lang="ts">
    import { portal } from '@zag-js/svelte';
    import { untrack } from 'svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { SettingsOutline } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import type { Status } from '~/lib/models/status';
    import { guardNull } from '~/lib/utils/guard';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { getActivityList, getStatuses, patchTaskStatus } from './page.remote';
    import { useActivityLists, usePageContext, useTask } from './utils.svelte';

    const pageData = usePageData<PageData>();
    const id = $props.id();
    const task = $derived(await useTask());
    const ctx = usePageContext();
    const activityLists = $derived(useActivityLists(ctx.activityListParams));
    const menu = createMenu({
        id,
        defaultHighlightedValue: untrack(() => task.status?.id),
        onOpenChange: async (details) => {
            if (details.open) {
                statuses = await getStatuses(pageData.project.id)
                    .run()
                    .then((a) => a.items);
                menu.api.setHighlightedValue(task.status?.id);
            }
        },
        onSelect: (details) => updateStatus(details.value),
    });

    async function updateStatus(statusId: string) {
        const status = statuses?.find((a) => a.id === statusId);
        guardNull(status);
        const lastList = activityLists.at(-1);
        guardNull(lastList);
        guardNull(lastList.query.current);
        const oldStatus = task.status;
        const optimisticActivity = {
            id: self.crypto.randomUUID(),
            actor: pageData.user,
            createdTime: new Date().toISOString(),
            kind: ActivityKind.StatusChanged,
            metadata: {
                status,
                oldStatus,
            },
        };
        await patchTaskStatus({
            taskId: task.id,
            statusId,
            version: task.version,
        }).updates(
            useTask().withOverride((task) => ({
                ...task,
                status,
            })),
            getActivityList(lastList.param).withOverride((list) => {
                return {
                    ...list,
                    items: [...list.items, optimisticActivity],
                };
            })
        );
    }

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
            {task.status?.name ?? 'No status'}
        </span>
        <SettingsOutline />
    </button>
    <div use:portal {...menu.api.getPositionerProps()}>
        <ul
            {...menu.api.getContentProps()}
            class="{C.menu({ part: 'content' })} flex flex-col gap-1 w-(--reference-width)"
        >
            {#each statuses as status (status.id)}
                <li>
                    <button
                        {...menu.api.getItemProps({ value: status.id })}
                        type="button"
                        class="{C.menu({ part: 'item' })} flex items-center gap-4 w-full"
                    >
                        {status.name}
                    </button>
                </li>
            {/each}
        </ul>
    </div>
</div>
