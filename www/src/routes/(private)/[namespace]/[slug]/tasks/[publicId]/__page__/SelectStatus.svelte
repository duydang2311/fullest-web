<script lang="ts">
    import { portal } from '@zag-js/svelte';
    import { untrack } from 'svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { SettingsOutline } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import { guardNull } from '~/lib/utils/guard';
    import { usePageData } from '~/lib/utils/kit';
    import { getStatusIcon } from '~/lib/utils/status';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { getActivityList, getStatuses, patchTaskStatus } from './page.remote';
    import { useActivityLists, usePageContext, useStatusList, useTask } from './utils.svelte';

    const pageData = usePageData<PageData>();
    const id = $props.id();
    const ctx = usePageContext();
    const activityLists = $derived(useActivityLists(ctx.activityListParams));
    const task = $derived(await useTask());
    let statusList = $state.raw<Awaited<ReturnType<typeof useStatusList>>>();
    const menu = createMenu({
        id,
        defaultHighlightedValue: untrack(() => task.status?.id),
        onOpenChange: async (details) => {
            if (details.open) {
                statusList ??= await getStatuses(pageData.project.id).run();
                menu.api.setHighlightedValue(task.status?.id);
            }
        },
        onSelect: (details) => updateStatus(details.value),
    });

    async function updateStatus(statusId: string) {
        guardNull(statusList);
        const status = statusList.items.find((a) => a.id === statusId);
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
            {#each statusList?.items as status (status.id)}
                {@const Icon = getStatusIcon(status)}
                <li>
                    <button
                        {...menu.api.getItemProps({ value: status.id })}
                        type="button"
                        class="{C.menu({ part: 'item' })} flex items-center gap-2 w-full"
                    >
                        <Icon />
                        {status.name}
                    </button>
                </li>
            {/each}
        </ul>
    </div>
</div>
