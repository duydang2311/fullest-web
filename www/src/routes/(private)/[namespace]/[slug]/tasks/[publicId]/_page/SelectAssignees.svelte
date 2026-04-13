<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import { liteDebounce } from '@tanstack/pacer-lite';
    import { ListCollection } from '@zag-js/collection';
    import { portal } from '@zag-js/svelte';
    import invariant from 'tiny-invariant';
    import Avatar from '~/lib/components/Avatar.svelte';
    import { createListbox, createPopover } from '~/lib/components/builders.svelte';
    import { SettingsOutline } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { assignTask, searchUsers, unassignTask } from './page.remote';
    import { usePageContext } from './utils.svelte';

    const data = usePageData<PageData>();
    const ctx = usePageContext();
    const id = $props.id();
    let searchQuery = $state.raw('');
    let users = $derived(
        searchQuery ? await searchUsers({ query: searchQuery }).then((a) => a.items) : []
    );
    const popover = createPopover({
        id: `popover-${id}`,
        onOpenChange: async (details) => {
            if (details.open) {
                // users = await getPriorities(data.project.id).then((a) => a.items);
            }
        },
    });
    const assigneeIds = $derived.by(() => new Set(ctx.task.assignees.map((a) => a.id)));
    const listboxUsers = $derived([
        ...ctx.task.assignees,
        ...(users ?? []).filter((a) => !assigneeIds.has(a.id)),
    ]);
    const listbox = createListbox({
        id: `listbox-${id}`,
        selectionMode: 'multiple',
        get collection() {
            return new ListCollection({
                items: listboxUsers ?? [],
                itemToValue: (a) => a.id,
            });
        },
        get value() {
            return ctx.task.assignees.map((a) => a.id);
        },
        onValueChange: (details) => {
            const assignees = new Set(ctx.task.assignees.map((a) => a.id));
            const current = new Set(details.value);
            const assigned = current.difference(assignees);
            const unassigned = assignees.difference(current);

            updateAssignees({ assigned, unassigned });
        },
    });

    async function updateAssignees({
        assigned,
        unassigned,
    }: {
        assigned: Set<string>;
        unassigned: Set<string>;
    }) {
        const users = $state.snapshot(listboxUsers);

        const oldTask = $state.snapshot(ctx.task);
        const oldActivityList = $state.snapshot(ctx.activityList);
        invariant(oldTask, 'oldTask must not be null');
        invariant(oldActivityList, 'oldActivityList must not be null');

        ctx.activityList = {
            ...oldActivityList,
            items: [
                ...oldActivityList.items,
                ...Array.from(assigned).map((a) => {
                    const user = users.find((b) => b.id === a);
                    invariant(user, 'user must not be null');
                    return {
                        id: self.crypto.randomUUID(),
                        actor: data.user,
                        createdTime: new Date().toISOString(),
                        kind: ActivityKind.Assigned,
                        metadata: {
                            assignee: user,
                        },
                    };
                }),
                ...Array.from(unassigned).map((a) => {
                    const user = users.find((b) => b.id === a);
                    invariant(user, 'user must not be null');
                    return {
                        id: self.crypto.randomUUID(),
                        actor: data.user,
                        createdTime: new Date().toISOString(),
                        kind: ActivityKind.Unassigned,
                        metadata: {
                            assignee: user,
                        },
                    };
                }),
            ],
        };
        ctx.task = {
            ...oldTask,
            assignees: [
                ...ctx.task.assignees.filter((a) => unassigned.has(a.id)),
                ...users.filter((a) => assigned.has(a.id)),
            ],
        };
        await Promise.all([
            ...Array.from(assigned).map((userId) =>
                assignTask({
                    taskId: ctx.task.id,
                    userId,
                })
            ),
            ...Array.from(unassigned).map((userId) =>
                unassignTask({
                    taskId: ctx.task.id,
                    userId,
                })
            ),
        ]).finally(invalidateAll);
    }
    const updateSearchQueryDebounced = liteDebounce(
        (value: string) => {
            searchQuery = value;
        },
        {
            wait: 200,
        }
    );
</script>

<div class="text-sm">
    <button
        {...popover.api.getTriggerProps()}
        type="button"
        class="{C.button({
            variant: 'base',
            ghost: true,
        })} text-left font-medium w-full flex items-center max-lg:flex-row-reverse max-lg:justify-end gap-2 lg:justify-between"
    >
        <span
            class="lg:hidden ml-auto bg-primary rounded-sm size-5 leading-none text-xs font-bold flex justify-center items-center"
        >
            {ctx.task.assignees.length}
        </span>
        <span>Assignees</span>
        <SettingsOutline />
    </button>
    {#if ctx.task.assignees.length}
        <ul class="mt-2 flex flex-col gap-2 px-2 max-lg:hidden">
            {#each ctx.task.assignees as assignee (assignee.id)}
                <div class="flex items-center gap-2">
                    <Avatar
                        user={assignee}
                        class="min-w-avatar-xs size-avatar-xs rounded-full border border-surface-border"
                    />
                    <div>{assignee.displayName ?? assignee.name}</div>
                </div>
            {/each}
        </ul>
    {/if}
    <div use:portal {...popover.api.getPositionerProps()}>
        <div
            {...popover.api.getContentProps()}
            class="{C.menu({ part: 'content' })} w-(--reference-width) p-0 min-h-48"
        >
            <input
                type="input"
                class="{C.input()} border-0 border-b border-b-base-border bg-surface-subtle rounded-b-none focus:ring-0"
                placeholder="Search users..."
                oninput={(e) => {
                    if (e.currentTarget.value.length === 0) {
                        users = [];
                    } else {
                        updateSearchQueryDebounced(e.currentTarget.value);
                    }
                }}
            />
            {#if listboxUsers.length}
                <ul class="flex flex-col gap-1 p-1">
                    {#each listboxUsers as user (user.id)}
                        <li>
                            <button
                                {...listbox.api.getItemProps({ item: user })}
                                type="button"
                                class="{C.menu({ part: 'item' })} flex items-center gap-2 w-full"
                            >
                                <Avatar
                                    {user}
                                    class="size-avatar-xs rounded-full border border-surface-border"
                                />
                                <span>
                                    {user.displayName ?? user.name}
                                </span>
                            </button>
                        </li>
                    {/each}
                </ul>
            {:else}
                <div class="c-help-text p-2">No users found.</div>
            {/if}
        </div>
    </div>
</div>
