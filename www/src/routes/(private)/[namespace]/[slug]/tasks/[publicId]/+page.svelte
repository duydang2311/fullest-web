<script lang="ts">
    import { watch } from '@duydang2311/svutils';
    import { untrack } from 'svelte';
    import { useRuntime } from '~/lib/utils/runtime';
    import ActivitySection from './_page/ActivitySection.svelte';
    import SelectAssignees from './_page/SelectAssignees.svelte';
    import SelectPriority from './_page/SelectPriority.svelte';
    import SelectStatus from './_page/SelectStatus.svelte';
    import Task from './_page/Task.svelte';
    import { fetchActivityList, setPageContext } from './_page/utils.svelte';

    const { data } = $props();
    const { http } = useRuntime();
    const ctx = setPageContext({ task: untrack(() => data.task) });

    watch(() => data)(() => {
        ctx.task = data.task;
        fetchActivityList(http)(data.task.id, null, ctx.activityList?.items.length).then((list) => {
            ctx.activityList = list;
        });
    });
</script>

<div class="divide-base-border-weak mx-auto w-full flex-1 lg:flex lg:divide-x max-h-full">
    <div class="flex-1 py-4">
        <Task />
        <div class="mt-8 px-4 lg:px-8">
            <ActivitySection />
        </div>
    </div>
    <div class="hidden lg:block w-64 h-fit sticky top-0">
        <div class="text-sm text-base-fg-dim font-medium border-b border-b-surface-border p-4">
            Properties
        </div>
        <div class="flex flex-col gap-2 p-4">
            <SelectStatus />
            <SelectPriority />
            <SelectAssignees />
        </div>
    </div>
</div>
