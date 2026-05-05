<script lang="ts">
    import { page } from '$app/state';
    import { boolAttr } from 'runed';
    import { untrack } from 'svelte';
    import Avatar from '~/lib/components/Avatar.svelte';
    import { createCollapsible } from '~/lib/components/builders.svelte';
    import {
        ChevronDownOutline,
        IconDoubleArrowDownOutline,
        IconUserCircleDashedOutline,
        PlusOutline,
    } from '~/lib/components/icons';
    import type { KeysetList } from '~/lib/models/paginated';
    import type { Status } from '~/lib/models/status';
    import { Flip, gsap } from '~/lib/utils/gsap';
    import { guardNull } from '~/lib/utils/guard';
    import { usePageData } from '~/lib/utils/kit';
    import { getPriorityIcon } from '~/lib/utils/priority';
    import { useRuntime } from '~/lib/utils/runtime';
    import { getStatusColor, getStatusIcon } from '~/lib/utils/status';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { makeFetchTaskList, usePageContext, type LocalTask } from './utils.svelte';

    interface Props {
        status: Pick<Status, 'id' | 'color' | 'category' | 'name'>;
        list: KeysetList<LocalTask>;
    }

    gsap.registerPlugin(Flip);

    const { status, list }: Props = $props();
    const id = $props.id();
    const { http } = useRuntime();
    const collapsible = createCollapsible({
        id,
        defaultOpen: untrack(() => list.items.length > 0),
    });
    const fetchTaskList = makeFetchTaskList(http);
    const ctx = usePageContext();
    const pageData = usePageData<PageData>();
    const Icon = $derived(getStatusIcon(status));

    async function loadMore(direction: 'forward') {
        if (direction === 'forward') {
            const lastItem = list.items.at(-1);
            guardNull(lastItem);
            const group = ctx.taskGroups[status.id];
            guardNull(group);
            const taskList = await fetchTaskList({
                projectId: pageData.project.id,
                statusId: status.id === 'none' ? null : status.id,
                hasStatusFilter: true,
                afterId: lastItem.id,
                direction: 'desc',
                size: 10,
            });
            group.hasNext = taskList.hasNext;
            group.items.push(...taskList.items);
        }
    }
</script>

<div
    {...collapsible.api.getRootProps()}
    class="group rootEl col-span-full grid grid-cols-subgrid first:rounded-t-md last:rounded-b-md overflow-hidden"
>
    <button
        {...collapsible.api.getTriggerProps()}
        type="button"
        class="relative px-4 py-2 bg-surface-subtle hover:bg-surface-emph transition col-span-full group-not-first:border-t group-not-first:border-t-surface-border group-data-[state=open]:border-b group-last:group-data-[state=closed]:border-b-0 border-b-surface-border tracking-tight flex items-center gap-4"
    >
        <Icon class="text-fg-muted group-data-[state=open]:text-fg transition duration-200" />
        <div
            class="gap-2 items-center text-fg-muted group-data-[state=open]:text-fg transition duration-200 flex"
        >
            <span>{status.name}</span>
            {#if list.totalCount > 0}
                <span class="bg-base-emph px-1 block text-xs font-bold rounded-sm">
                    {list.totalCount}
                </span>
            {/if}
        </div>
        <div class="ml-auto flex gap-2 items-center">
            <a
                href="/{page.params.namespace}/{page.params.slug}/tasks/new"
                class={C.button({ ghost: true, size: 'sm', icon: true })}
                onclick={(e) => {
                    e.stopPropagation();
                }}
                title="Create task"
            >
                <PlusOutline />
            </a>
            <ChevronDownOutline
                class="transition-transform group-data-[state=open]:-rotate-180 duration-200 text-fg-muted"
            />
        </div>
        <div
            class="absolute h-[calc(100%+2px)] w-1 group-data-[state=closed]:-translate-x-full transition-transform origin-left left-0 -top-px"
            style="background-color: {getStatusColor(status)}"
        ></div>
    </button>
    <div {...collapsible.api.getContentProps()} class="col-span-full grid grid-cols-subgrid">
        <div class="relative col-span-full grid grid-cols-subgrid text-xs text-fg-muted">
            <div class="px-4 py-2">#</div>
            <div class="px-4 py-2">Name</div>
            <div class="px-4 py-2">Assign</div>
            <div class="px-4 py-2">Priority</div>
            <div
                class="absolute h-[calc(100%+2px)] w-1 scale-x-0 group-data-[state=open]:scale-x-100 transition-transform origin-left left-0 -top-px"
                style="background-color: {getStatusColor(status)}"
            ></div>
        </div>
        {#each list.items as item (item.id)}
            {@const PriorityIcon = getPriorityIcon(item.priority)}
            <div
                class="group/item relative col-span-full grid grid-cols-subgrid hover:bg-surface-subtle items-center"
            >
                <a
                    href="/{page.params.namespace}/{page.params.slug}/tasks/{item.publicId}"
                    class="absolute inset-0"
                    aria-label="Go to task"
                >
                </a>
                <div
                    class="absolute h-full w-1 group-hover/item:opacity-100 opacity-40 left-0 top-0 transition hover:duration-0"
                    style="background-color: {getStatusColor(status)}"
                ></div>
                <div class="px-4 py-2 text-sm font-medium text-fg-muted content-center">
                    {item.publicId}
                </div>
                <div class="px-4 py-2">{item.title}</div>
                <div class="px-4 py-2 flex gap-1">
                    <button
                        type="button"
                        class="{C.button({ ghost: true, icon: true, size: 'sm' })} relative"
                        onclick={(e) => {}}
                    >
                        {#if item.assignees != null && item.assignees.length > 0}
                            <div class="flex items-center">
                                {#each item.assignees as assignee (assignee.name)}
                                    <Avatar
                                        user={assignee}
                                        class="size-avatar-xs outline-2 outline-surface shrink-0 -ml-2 first:ml-0"
                                    />
                                {/each}
                            </div>
                        {:else}
                            <IconUserCircleDashedOutline />
                        {/if}
                    </button>
                </div>
                <div class="px-4 py-2 ml-auto">
                    <PriorityIcon
                        data-no-priority={boolAttr(item.priority == null)}
                        class="data-no-priority:text-fg-muted"
                    />
                </div>
            </div>
        {/each}
        {#if list.hasNext}
            <button
                type="button"
                class="{C.button({
                    variant: 'surface',
                    filled: true,
                })} not-hover:text-fg-muted relative rounded-none col-span-full flex items-center gap-2 justify-center text-xs uppercase tracking-wide border-t border-t-surface-border"
                onclick={() => {
                    loadMore('forward');
                }}
            >
                <span>Show more</span>
                <IconDoubleArrowDownOutline />
            </button>
        {/if}
    </div>
</div>
