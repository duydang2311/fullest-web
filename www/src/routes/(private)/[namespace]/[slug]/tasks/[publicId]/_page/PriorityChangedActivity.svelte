<script lang="ts">
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { Priority } from '~/lib/models/priority';
    import type { User } from '~/lib/models/user';
    import { namespaceUrl } from '~/lib/utils/url';
    import ActivityAvatar from './ActivityAvatar.svelte';

    const {
        activity,
    }: {
        activity: Pick<Activity, 'createdTime'> & {
            actor: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
            metadata: {
                priority: Pick<Priority, 'name'> | null;
                oldPriority: Pick<Priority, 'name'> | null;
            };
        };
    } = $props();

    const metadata = $derived(activity.metadata);
</script>

{#if metadata}
    <div class="flex gap-2 items-center px-2">
        <ActivityAvatar user={activity.actor} />
        <div>
            <a href={namespaceUrl(activity.actor.name)} class="c-link">
                <strong>{activity.actor.displayName ?? activity.actor.name}</strong>
            </a>
            <span class="text-sm">
                <span>changed the priority to</span>
                <span class="px-2 bg-surface-emph rounded-sm">
                    {metadata.priority?.name ?? 'None'}
                </span>
                <span class="text-sm font-medium text-fg-dim">
                    · <RelativeDateTime time={activity.createdTime} />
                </span>
            </span>
        </div>
        <div
            class="ml-auto px-2 bg-surface-subtle rounded-sm text-sm line-through text-fg-muted"
        >
            {metadata.oldPriority?.name ?? 'None'}
        </div>
    </div>
{/if}
