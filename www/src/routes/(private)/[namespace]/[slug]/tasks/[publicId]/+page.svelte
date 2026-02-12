<script lang="ts">
    import invariant from 'tiny-invariant';
    import { ActivityKind } from '~/lib/models/activity';
    import ActivitySection from './_page/ActivitySection.svelte';
    import { getActivities } from './_page/page.remote';
    import SelectAssignees from './_page/SelectAssignees.svelte';
    import SelectPriority from './_page/SelectPriority.svelte';
    import SelectStatus from './_page/SelectStatus.svelte';
    import Task from './_page/Task.svelte';

    const { data } = $props();
    const activities = $derived(getActivities(data.task.id));
    const filteredActivities = $derived(
        activities.current?.items.filter((a) => {
            if (a.kind !== ActivityKind.Commented) {
                return true;
            }
            invariant(
                typeof a.data === 'object' &&
                    a.data &&
                    'comment' in a.data &&
                    a.data.comment &&
                    typeof a.data.comment === 'object' &&
                    a.data.comment &&
                    'id' in a.data.comment &&
                    typeof a.data.comment.id === 'string',
                'activity.data.comment.id must be string'
            );
            return a.data.comment.id !== data.task.initialCommentId;
        }) ?? []
    );
</script>

<div class="divide-base-border-weak mx-auto w-full flex-1 lg:flex lg:divide-x max-h-full">
    <div class="flex-1 py-4">
        <Task task={data.task} comment={data.task.initialComment} />
        <div class="mt-8 px-4 lg:px-8">
            <ActivitySection
                user={data.user}
                taskId={data.task.id}
                activities={filteredActivities}
            />
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
