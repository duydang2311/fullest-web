<script lang="ts">
    import { page } from '$app/state';
    import { untrack } from 'svelte';
    import { MagnifyingGlass, PlusOutline } from '~/lib/components/icons';
    import { button } from '~/lib/utils/styles';
    import StatusGroup from './StatusGroup.svelte';

    const { data } = $props();
    const statusNames = untrack(() =>
        Object.fromEntries(data.statusList.items.map((a) => [a.id, a.name]))
    );
    const tasks = untrack(() =>
        data.taskList
            ?.map((a) => Object.groupBy(a.items, (b) => b.status?.id ?? 'null'))
            .reduce(
                (acc, cur) => {
                    console.log(cur);
                    for (const k of Object.keys(cur)) {
                        acc[k ?? 'null'] = {
                            name: statusNames[k],
                            items: cur[k] ?? [],
                        };
                    }
                    return acc;
                },
                {} as Record<
                    string,
                    { name: string; items: (typeof data.taskList)[number]['items'] }
                >
            )
    );
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
                <MagnifyingGlass class="absolute left-2 top-1/2 -translate-y-1/2 text-fg-muted" />
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
        <div class="px-8">
            <div
                class="grid grid-cols-[auto_1fr_auto_auto] border border-surface-border rounded-md"
            >
                {#each [{ id: 'null', name: 'null' }, ...data.statusList.items] as item (item.id)}
                    <StatusGroup
                        name={item.name === 'null' ? 'No status' : item.name}
                        items={tasks[item.id]?.items ?? []}
                    />
                {/each}
            </div>
        </div>
    </div>
</div>
