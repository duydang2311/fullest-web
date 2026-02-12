<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import { LiteDebouncer } from '@tanstack/pacer-lite';
    import { ListCollection } from '@zag-js/collection';
    import { portal } from '@zag-js/svelte';
    import Avatar from '~/lib/components/Avatar.svelte';
    import { createListbox, createPopover } from '~/lib/components/builders.svelte';
    import { SettingsOutline } from '~/lib/components/icons';
    import type { User, UserPreset } from '~/lib/models/user';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { usePageData } from './context.svelte';
    import { assignTask, searchUsers, unassignTask } from './page.remote';

    const data = usePageData<PageData>();
    const id = $props.id();
    let users = $state.raw<(Pick<User, 'id'> & UserPreset['Avatar'])[]>();
    const popover = createPopover({
        id: `popover-${id}`,
        onOpenChange: async (details) => {
            if (details.open) {
                // users = await getPriorities(data.project.id).then((a) => a.items);
            }
        },
    });
    const assigneeIds = $derived.by(() => new Set(data.task.assignees.map((a) => a.id)));
    const listboxUsers = $derived([
        ...data.task.assignees,
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
            return data.task.assignees.map((a) => a.id);
        },
        onValueChange: async (details) => {
            const assignees = new Set(data.task.assignees.map((a) => a.id));
            const current = new Set(details.value);
            let needInvalidate = false;
            for (const userId of current.difference(assignees)) {
                needInvalidate = true;
                await assignTask({
                    taskId: data.task.id,
                    userId: userId,
                });
            }
            for (const userId of assignees.difference(current)) {
                needInvalidate = true;
                await unassignTask({
                    taskId: data.task.id,
                    userId: userId,
                });
            }
            if (needInvalidate) {
                await invalidateAll();
            }
        },
    });
    const debouncedSearchUser = new LiteDebouncer(
        async (value: string) => {
            users = await searchUsers({ query: value }).then((a) => a.items);
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
            {data.task.assignees.length}
        </span>
        <span>Assignees</span>
        <SettingsOutline />
    </button>
    {#if data.task.assignees.length}
        <ul class="mt-2 flex flex-col gap-2 px-2 max-lg:hidden">
            {#each data.task.assignees as assignee (assignee.id)}
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
                        debouncedSearchUser.cancel();
                        users = [];
                    } else {
                        debouncedSearchUser.maybeExecute(e.currentTarget.value);
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
