<script lang="ts">
    import type { Activity } from '~/lib/models/activity';
    import type { User, UserPreset } from '~/lib/models/user';
    import ActivityItem from './ActivityItem.svelte';
    import AddComment from './AddComment.svelte';

    const {
        taskId,
        activities,
        user,
    }: {
        taskId: string;
        activities: (Pick<Activity, 'id' | 'createdTime' | 'kind' | 'data'> & {
            actor: Pick<User, 'id'> & UserPreset['Avatar'];
        })[];
        user: UserPreset['Avatar'];
    } = $props();
</script>

<section class="mt-8 max-w-container-lg mx-auto">
    {#if activities.length > 0}
        <ol class="mt-4 flex flex-col gap-6 animate-slide-in-from-b duration-300 slide-[1rem]">
            {#each activities as activity (activity.id)}
                <ActivityItem {activity} />
            {/each}
        </ol>
    {/if}
    <div class="mt-6">
        <AddComment {user} {taskId} />
    </div>
</section>
