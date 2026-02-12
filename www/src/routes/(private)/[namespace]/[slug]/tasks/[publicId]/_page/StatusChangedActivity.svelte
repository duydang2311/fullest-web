<script lang="ts">
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { Status } from '~/lib/models/status';
    import type { User } from '~/lib/models/user';
    import ActivityAvatar from './ActivityAvatar.svelte';
    import { namespaceUrl } from '~/lib/utils/url';

    const {
        activity,
    }: {
        activity: Pick<Activity, 'createdTime' | 'data'> & {
            actor: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
            data: {
                status?: Pick<Status, 'name'>;
                oldStatus?: Pick<Status, 'name'>;
            };
        };
    } = $props();

    const data = $derived(activity.data);
</script>

{#if data}
    <div class="flex gap-2 items-center px-2">
        <ActivityAvatar user={activity.actor} />
        <div>
            <a href={namespaceUrl(activity.actor.name)} class="c-link">
                <strong>{activity.actor.displayName ?? activity.actor.name}</strong>
            </a>
            <span class="text-sm">
                <span>changed the status to</span>
                <span class="px-2 bg-surface-emph rounded-sm">
                    {data.status?.name ?? 'None'}
                </span>
                <span class="text-sm font-medium text-base-fg-dim">
                    · <RelativeDateTime time={activity.createdTime} />
                </span>
            </span>
        </div>
        <div
            class="ml-auto px-2 bg-surface-subtle rounded-sm text-sm line-through text-base-fg-muted"
        >
            {data.oldStatus?.name ?? 'None'}
        </div>
    </div>
{/if}
