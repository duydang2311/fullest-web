<script lang="ts">
    import { ActivityKind } from '~/lib/models/activity';
    import ActivitySection from './_page/ActivitySection.svelte';
    import Stats from './_page/Stats.svelte';
    import Task from './_page/Task.svelte';
    import { getActivities } from './_page/page.remote';

    const { data } = $props();
    const activities = $derived(getActivities(data.task.id));
    const filteredActivities = $derived(
        activities.current?.items.filter(
            (a) =>
                a.kind !== ActivityKind.Commented ||
                a.data?.comment.id !== data.task.initialCommentId
        ) ?? []
    );
</script>

<div
    class="divide-base-border-weak max-w-container-xl mx-auto w-full flex-1 *:py-4 lg:flex lg:divide-x"
>
    <div class="flex-1 px-8">
        <Task task={data.task} comment={data.task.initialComment} />
        <div class="mt-8">
            <ActivitySection
                user={data.user}
                taskId={data.task.id}
                activities={filteredActivities}
            />
        </div>
    </div>
    <div class="w-64 lg:pl-8">
        <div class="hidden flex-col gap-4 lg:flex">
            <Stats />
        </div>
    </div>
</div>
