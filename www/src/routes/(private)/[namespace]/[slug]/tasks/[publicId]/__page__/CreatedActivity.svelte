<script lang="ts">
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { User } from '~/lib/models/user';
    import { namespaceUrl } from '~/lib/utils/url';
    import ActivityAvatar from './ActivityAvatar.svelte';

    const {
        activity,
    }: {
        activity: Pick<Activity, 'createdTime'> & {
            actor: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
        };
    } = $props();
</script>

<div class="flex gap-2 items-center px-2">
    <ActivityAvatar user={activity.actor} />
    <div>
        <a href={namespaceUrl(activity.actor.name)} class="c-link">
            <strong>{activity.actor.displayName ?? activity.actor.name}</strong>
        </a>
        <span class="text-sm">
            <span>created the task</span>
            <span class="text-sm font-medium text-fg-dim">
                · <RelativeDateTime time={activity.createdTime} />
            </span>
        </span>
    </div>
</div>
