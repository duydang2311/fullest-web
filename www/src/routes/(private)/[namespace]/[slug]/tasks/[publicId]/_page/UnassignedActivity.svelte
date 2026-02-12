<script lang="ts">
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { User, UserPreset } from '~/lib/models/user';
    import { namespaceUrl } from '~/lib/utils/url';
    import ActivityAvatar from './ActivityAvatar.svelte';

    const {
        activity,
    }: {
        activity: Pick<Activity, 'createdTime' | 'data'> & {
            actor: Pick<User, 'id'> & UserPreset['Avatar'];
            data: { assignee: Pick<User, 'id' | 'name' | 'displayName'> };
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
                {#if data.assignee.id === activity.actor.id}
                    <span>unassigned themselves</span>
                {:else}
                    <span>
                        unassigned
                        {data.assignee.displayName ?? data.assignee.name}
                    </span>
                {/if}
                <span class="text-sm font-medium text-base-fg-dim">
                    · <RelativeDateTime time={activity.createdTime} />
                </span>
            </span>
        </div>
    </div>
{/if}
