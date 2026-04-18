<script lang="ts">
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { User, UserPreset } from '~/lib/models/user';
    import { namespaceUrl } from '~/lib/utils/url';
    import ActivityAvatar from './ActivityAvatar.svelte';

    const {
        activity,
    }: {
        activity: Pick<Activity, 'createdTime'> & {
            actor: Pick<User, 'id'> & UserPreset['Avatar'];
            metadata: { assignee: Pick<User, 'id' | 'name' | 'displayName'> };
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
                {#if metadata.assignee.id === activity.actor.id}
                    <span>self-assigned</span>
                {:else}
                    <span>
                        assigned
                        {metadata.assignee.displayName ?? metadata.assignee.name}
                    </span>
                {/if}
                <span class="text-sm font-medium text-fg-dim">
                    · <RelativeDateTime time={activity.createdTime} />
                </span>
            </span>
        </div>
    </div>
{/if}
