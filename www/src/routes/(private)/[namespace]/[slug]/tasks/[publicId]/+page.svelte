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

    const ctx = setPageContext({
        activityListParams: [],
    });
    const task = $derived(await useTask());

    ctx.activityListParams.push({ taskId: untrack(() => task.id), size: 20 });
</script>

<div class="divide-surface-border mx-auto w-full flex-1 lg:flex lg:divide-x max-h-full">
    <div class="flex-1 py-4">
        <Task />
        <div class="max-w-container-lg mx-auto">
            <hr class="border-surface-border mt-4" />
        </div>
        <div class="mt-8 px-8">
            <div class="max-w-container-lg mx-auto">
                <ActivitySection />
                <div class="mt-6">
                    <AddComment />
                </div>
            </div>
        </div>
    </div>
    <div class="hidden lg:block w-72 h-fit sticky top-0 divide-y divide-surface-border">
        <div class="p-4">
            <h2 class="text-sm text-fg-emph font-semibold">Properties</h2>
            <div class="flex flex-col gap-2 mt-2">
                <SelectStatus />
                <SelectPriority />
                <SelectAssignees />
            </div>
        </div>
        <div class="p-4">
            <h2 class="text-sm text-fg-emph font-semibold">Actions</h2>
            <div class="flex flex-col gap-2 mt-2">
                <DeleteTask />
            </div>
        </div>
    </div>
</div>
