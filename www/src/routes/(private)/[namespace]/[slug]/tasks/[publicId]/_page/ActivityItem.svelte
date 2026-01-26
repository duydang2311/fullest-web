<script lang="ts">
    import { ActivityKind, type Activity } from '~/lib/models/activity';
    import type { User } from '~/lib/models/user';
    import CommentedActivity from './CommentedActivity.svelte';
    import CreatedActivity from './CreatedActivity.svelte';

    const {
        taskId,
        activity,
    }: {
        taskId: string;
        activity: Pick<Activity, 'id' | 'createdTime' | 'kind' | 'data'> & {
            actor: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
        };
    } = $props();
</script>

{#if activity.kind === ActivityKind.Created}
    <CreatedActivity {activity} />
{:else if activity.kind === ActivityKind.Commented}
    <CommentedActivity {taskId} {activity} />
{/if}
