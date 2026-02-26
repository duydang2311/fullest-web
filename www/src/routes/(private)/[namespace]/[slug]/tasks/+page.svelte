<script lang="ts">
    import { page } from '$app/state';
    import Avatar from '~/lib/components/Avatar.svelte';
    import { MagnifyingGlass, PlusOutline } from '~/lib/components/icons';
    import { button } from '~/lib/utils/styles';

    const { data } = $props();
</script>

<div class="mx-auto w-full">
    <!-- <div class="flex items-center justify-between gap-4">
        <div class="w-lg relative max-w-full">
            <input type="text" placeholder="Search tasks..." class="c-input w-full pl-8" />
            <MagnifyingGlass class="absolute left-2 top-1/2 -translate-y-1/2" />
        </div>
        <a
            href="/{page.params.namespace}/{page.params.slug}/tasks/new"
            class="{button({
                variant: 'primary',
                filled: true,
                outlined: true,
            })} flex items-center gap-2 text-nowrap"
        >
            <PlusOutline />
            New Task
        </a>
    </div> -->
    <div class="h-16 flex gap-4 border-b border-b-surface-border">
        <h1 class="text-title-xs tracking-tight font-bold content-center text-fg-emph pl-8">
            Tasks
        </h1>
    </div>
    <!-- <div
        class="flex *:flex-1 divide-x divide-surface-border bg-surface-subtle border-b-surface-border border-b"
    >
        <div class="h-36 content-center px-8">
            <div class="text-fg-muted tracking-wide text-sm">PROGRESS</div>
            <div class="text-4xl font-light">92%</div>
        </div>
        <div class="h-36 content-center px-8">
            <div class="text-fg-muted tracking-wide text-sm">OPEN ITEMS</div>
            <div class="text-4xl font-light">42</div>
        </div>
        <div class="h-36 content-center px-8">
            <div class="text-fg-muted tracking-wide text-sm">DUE DATE</div>
            <div class="text-4xl font-light">84</div>
        </div>
        <div class="h-36 content-center px-8">
            <div class="text-fg-muted tracking-wide text-sm">TEAM</div>
            <div class="text-4xl font-light">10</div>
        </div>
    </div> -->
    <div>
        <div class="flex items-center gap-4 px-8 py-4">
            <div class="w-lg relative max-w-full h-fit my-auto">
                <input
                    type="text"
                    placeholder="Filter tasks..."
                    class="c-input bg-base border-0 max-w-64 pl-10"
                />
                <MagnifyingGlass
                    class="absolute left-2 top-1/2 -translate-y-1/2 text-fg-muted"
                />
            </div>
            <a
                href="/{page.params.namespace}/{page.params.slug}/tasks/new"
                class="{button({
                    variant: 'primary',
                    filled: true,
                })} ml-auto flex items-center gap-2 text-nowrap tracking-tight"
            >
                <PlusOutline />
                New Task
            </a>
        </div>
        <div class="overflow-auto">
            <table
                class="bg-surface min-w-container-md grid grid-cols-[auto_1fr_auto_auto_auto] content-start text-left"
            >
                <thead class="col-span-full grid grid-cols-subgrid">
                    <tr
                        class="border-b-surface-border col-span-full grid grid-cols-subgrid items-center border-b *:px-4 *:py-4 px-8"
                    >
                        <th class="text-fg-dim font-normal text-sm tracking-wide">ID</th>
                        <th class="text-fg-dim font-normal text-sm tracking-wide">Task</th>
                        <th class="text-fg-dim font-normal text-sm tracking-wide">
                            Author
                        </th>
                        <th class="text-fg-dim font-normal text-sm tracking-wide">
                            Status
                        </th>
                        <th class="text-fg-dim font-normal text-sm tracking-wide">
                            Priority
                        </th>
                    </tr>
                </thead>
                <tbody class="col-span-full grid grid-cols-subgrid">
                    {#each data.taskList.items as task (task.id)}
                        <tr
                            class="col-span-full items-center grid grid-cols-subgrid *:px-4 *:py-6 px-8 border-b border-b-surface-border"
                        >
                            <td class="text-fg-muted text-sm tracking-wide">
                                #{task.publicId}
                            </td>
                            <td
                                class="hover:bg-surface-subtle active:bg-surface-emph relative transition min-w-max"
                            >
                                <a
                                    href="/{page.params.namespace}/{page.params
                                        .slug}/tasks/{task.publicId}"
                                    aria-label="View task details"
                                    class="absolute inset-0"
                                ></a>
                                <div>
                                    {task.title}
                                </div>
                            </td>
                            <td class="min-w-32">
                                <div class="flex items-center gap-2">
                                    <Avatar
                                        user={task.author}
                                        class="size-avatar-xs rounded-full"
                                    />
                                    <span>{task.author.displayName ?? task.author.name}</span>
                                </div>
                            </td>
                            <td class="min-w-32">
                                <span>{task.status?.name ?? 'No status'}</span>
                                <!-- <div
                                        class="border-base-border border-3 absolute bottom-0 right-0 size-0 border-l-transparent border-t-transparent"
                                    ></div> -->
                            </td>
                            <td class="min-w-32">
                                <span>{task.priority?.name ?? 'No priority'}</span>
                                <!-- <div
                                        class="border-base-border border-3 absolute bottom-0 right-0 size-0 border-l-transparent border-t-transparent"
                                    ></div> -->
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    </div>
</div>
