<script lang="ts">
    import { untrack } from 'svelte';
    import ActivitySection from './__page__/ActivitySection.svelte';
    import SelectAssignees from './__page__/SelectAssignees.svelte';
    import SelectPriority from './__page__/SelectPriority.svelte';
    import SelectStatus from './__page__/SelectStatus.svelte';
    import Task from './__page__/Task.svelte';
    import { setPageContext, useTask } from './__page__/utils.svelte';

    const ctx = setPageContext({
        activityListParams: [],
    });
    const task = $derived(await useTask());

    ctx.activityListParams.push({ taskId: untrack(() => task.id), size: 20 });
</script>

<div class="divide-surface-border mx-auto w-full flex-1 lg:flex lg:divide-x max-h-full">
    <div class="flex-1 py-4">
        <Task />
        <div class="mt-8 px-8">
            <ActivitySection />
        </div>
    </div>
    <div class="hidden lg:block w-72 h-fit sticky top-0">
        <div class="text-sm text-fg-dim font-medium border-b border-b-surface-border p-4">
            Properties
        </div>
        <div class="flex flex-col gap-2 p-4">
            <SelectStatus />
            <SelectPriority />
            <SelectAssignees />
        </div>
    </div>
</div>
