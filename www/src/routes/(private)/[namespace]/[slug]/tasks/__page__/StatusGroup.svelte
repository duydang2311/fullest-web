<script lang="ts">
    import { page } from '$app/state';
    import { boolAttr } from 'runed';
    import { untrack } from 'svelte';
    import Avatar from '~/lib/components/Avatar.svelte';
    import { createCollapsible } from '~/lib/components/builders.svelte';
    import {
        ChevronRightOutline,
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
    class="group col-span-full grid grid-cols-subgrid overflow-hidden"
>
    <button
        {...collapsible.api.getTriggerProps()}
        type="button"
        class="rounded-md has-[a:hover]:bg-transparent *:opacity-60 *:group-data-[state=open]:opacity-100 hover:bg-base-subtle active:bg-base-emph relative transition col-span-full grid grid-cols-subgrid tracking-tight items-center"
    >
        <div class="p-2">
            <ChevronRightOutline
                class="transition-transform group-data-[state=open]:rotate-90 duration-200 text-fg-muted"
            />
        </div>
        <div class="p-2">
            <div
                class="size-2 rounded-full"
                style="background-color: {getStatusColor(status)}"
            ></div>
        </div>
        <div class="p-2 col-span-3 flex items-center gap-4">
            <Icon class="transition duration-200" style="color: {getStatusColor(status)}" />
            <div class="gap-2 items-center flex">
                <span class="transition">
                    {status.name}
                </span>
                {#if list.totalCount > 0}
                    <span
                        class="bg-base-emph text-fg-dim group-data-[state=open]:text-fg transition h-5 min-w-5 content-center text-center text-xs rounded-full"
                    >
                        {list.totalCount}
                    </span>
                {/if}
            </div>
        </div>
        <div class="flex gap-2 items-center p-2">
            <a
                href="/{page.params.namespace}/{page.params.slug}/tasks/new"
                class={C.button({ ghost: true, size: 'sm', icon: true })}
                title="Create task"
            >
                <PlusOutline />
            </a>
        </div>
    </button>
    <div {...collapsible.api.getContentProps()} class="col-span-full grid grid-cols-subgrid">
        {#each list.items as item (item.id)}
            {@const PriorityIcon = getPriorityIcon(item.priority)}
            <div
                class="group/item relative col-span-full grid grid-cols-subgrid hover:bg-base-subtle active:bg-base-emph items-center first:rounded-t-md last:rounded-b-md transition border-b last:border-none border-surface-border"
            >
                <a
                    href="/{page.params.namespace}/{page.params.slug}/tasks/{item.publicId}"
                    class="absolute inset-0"
                    aria-label="Go to task"
                >
                </a>
                <!-- <div
                    class="absolute h-full w-1 group-hover/item:opacity-100 opacity-40 left-0 top-0 transition hover:duration-0"
                    style="background-color: {getStatusColor(status)}"
                ></div> -->
                <div class="p-2 text-sm text-fg-muted content-center">
                    <!-- #{item.publicId} -->
                </div>
                <div class="p-2">
                    <div
                        class="size-2 rounded-[1px]"
                        style="background-color: {getStatusColor(status)}"
                    ></div>
                </div>
                <div class="p-2">
                    <Icon
                        class="text-fg-muted group-data-[state=open]:text-fg transition duration-200"
                        style="color: {getStatusColor(item?.status)}"
                    />
                </div>
                <div class="p-2">{item.title}</div>
                <div class="p-2">
                    <PriorityIcon
                        data-no-priority={boolAttr(item.priority == null)}
                        class="data-no-priority:text-fg-muted"
                    />
                </div>
                <div class="p-2">
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
