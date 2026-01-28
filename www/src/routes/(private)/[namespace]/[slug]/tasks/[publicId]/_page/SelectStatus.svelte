<script lang="ts">
    import { invalidate } from '$app/navigation';
    import { page } from '$app/state';
    import { portal } from '@zag-js/svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import type { Status } from '~/lib/models/status';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { usePageData } from './context.svelte';
    import { getStatuses, patchTaskStatus } from './page.remote';

    const data = usePageData<PageData>();
    const id = $props.id();
    const menu = createMenu({
        id,
        onOpenChange: async (details) => {
            if (details.open) {
                menu.api.setHighlightedValue(data.task.status?.id ?? '');
                statuses = await getStatuses(data.project.id).then((a) => a.items);
            }
        },
        onSelect: async (details) => {
            await patchTaskStatus({
                taskId: data.task.id,
                statusId: details.value,
            });
            await invalidate(page.route.id!);
        },
    });

    let statuses = $state.raw<Pick<Status, 'id' | 'name'>[]>();
</script>

<div>
    <button
        {...menu.api.getTriggerProps()}
        type="button"
        class="{C.button({
            variant: 'base',
            ghost: true,
            outlined: true,
        })} text-left c-help-text font-medium w-full"
    >
        {data.task.status?.name ?? 'No status'}
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
