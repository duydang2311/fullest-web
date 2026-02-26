<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import { portal } from '@zag-js/svelte';
    import invariant from 'tiny-invariant';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { SettingsOutline } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import type { Status } from '~/lib/models/status';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { getStatuses, patchTaskStatus } from './page.remote';
    import { usePageContext } from './utils.svelte';

    const data = usePageData<PageData>();
    const id = $props.id();
    const ctx = usePageContext();
    const menu = createMenu({
        id,
        onOpenChange: async (details) => {
            if (details.open) {
                menu.api.setHighlightedValue(ctx.task.status?.id ?? '');
                statuses = await getStatuses(data.project.id).run().then((a) => a.items);
            }
        },
        onSelect: (details) => updateStatus(details.value),
    });

    async function updateStatus(statusId: string) {
        const oldTask = $state.snapshot(ctx.task);
        const oldActivityList = $state.snapshot(ctx.activityList);
        invariant(oldTask, 'oldTask must not be null');
        invariant(oldActivityList, 'oldActivityList must not be null');

        const status = statuses?.find((a) => a.id === statusId);
        invariant(status, 'status must not be null');

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
                        status: {
                            id: statusId,
                            name: status.name,
                        },
                        oldStatus: {
                            id: oldTask.status?.id,
                            name: oldTask.status?.name ?? 'No status',
                        },
                    },
                },
            ],
        };
        ctx.task = {
            ...oldTask,
            status,
        };
        await patchTaskStatus({
            taskId: ctx.task.id,
            statusId,
        }).finally(invalidateAll);
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
            {ctx.task.status?.name ?? 'No status'}
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
