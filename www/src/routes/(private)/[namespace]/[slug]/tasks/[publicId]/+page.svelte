<script lang="ts">
    import { untrack } from 'svelte';
    import ActivitySection from './__page__/ActivitySection.svelte';
    import AddComment from './__page__/AddComment.svelte';
    import SelectAssignees from './__page__/SelectAssignees.svelte';
    import SelectPriority from './__page__/SelectPriority.svelte';
    import SelectStatus from './__page__/SelectStatus.svelte';
    import Task from './__page__/Task.svelte';
    import { setPageContext, useTask } from './__page__/utils.svelte';
    import DeleteTask from './__page__/DeleteTask.svelte';
    import Statistic from './__page__/Statistic.svelte';
    import { formatDate } from '~/lib/utils/date';

    const ctx = setPageContext({
        activityListParams: [],
    });
    const task = $derived(await useTask());

    ctx.activityListParams.push({ taskId: untrack(() => task.id), size: 20 });
</script>

<div
    class="divide-surface-border mx-auto max-w-container-lg w-full flex-1 lg:flex lg:divide-x max-h-full"
>
    <div class="flex-1 lg:pr-4">
        <Task />
        <!-- <hr class="border-surface-border mt-2" /> -->
        <div class="mt-8">
            <div>
                <ActivitySection />
                <div class="mt-6">
                    <AddComment />
                </div>
            </div>
        </div>
    </div>
    <div class="hidden lg:block w-72 h-fit sticky top-0 lg:pl-4">
    <h2 class="text-fg-muted font-medium text-body-xs">PROPERTIES</h2>
        <div class="flex flex-col gap-2 mt-2">
            <SelectStatus />
            <SelectPriority />
            <SelectAssignees />
        </div>
        <hr class="border-surface-border mt-2" />
        <div class="flex flex-col gap-2 mt-2">
            <Statistic label="Created" content={formatDate(task.createdTime)} />
            <Statistic label="Updated" content={formatDate(task.createdTime)} />
        </div>
        <hr class="border-surface-border mt-2" />
        <div class="flex flex-col gap-2 mt-2">
            <DeleteTask />
        </div>
    </div>
</div>
