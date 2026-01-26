<script lang="ts">
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { User } from '~/lib/models/user';
    import ActivityAvatar from './ActivityAvatar.svelte';

    const {
        activity,
    }: {
        activity: Pick<Activity, 'createdTime'> & {
            actor: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
        };
    } = $props();
</script>

<div class="flex gap-nbsp items-center px-2">
    <ActivityAvatar user={activity.actor} />
    <span>
        created the task
        <span class="text-sm font-medium text-base-fg-dim">
            · <RelativeDateTime time={activity.createdTime} />
        </span>
    </span>
</div>
