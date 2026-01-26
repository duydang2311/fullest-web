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
            actor: Pick<User, 'name'>;
        })[];
        user: UserPreset['Avatar'];
    } = $props();
</script>

<section>
    <h2 class="text-title-xs text-base-fg-strong font-semibold tracking-tight">Timeline</h2>
    {#if activities.length > 0}
        <ol class="mt-4 flex flex-col gap-6 animate-slide-in-from-b duration-300">
            {#each activities as activity (activity.id)}
                <li class="relative">
                    <ActivityItem {taskId} {activity} />
                    <div
                        class="absolute w-px h-6 left-0 bottom-0 translate-y-full translate-x-5.5 bg-base-border"
                    ></div>
                </li>
            {/each}
        </ol>
    {/if}
    <div class="mt-6">
        <AddComment {user} {taskId} />
    </div>
</section>
