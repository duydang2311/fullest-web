<script lang="ts">
    import type { ActivityKind } from '~/lib/models/activity';
    import { namespaceUrl } from '~/lib/utils/url';
    import type { InferOutput } from '~/lib/utils/validation';
    import type { activityValidators } from './utils.svelte';

    export const {
        activity,
    }: { activity: InferOutput<(typeof activityValidators)[ActivityKind.Unassigned]> } = $props();
</script>

<div>
    <a href={namespaceUrl(activity.actor.name)} class="c-link font-medium">
        {activity.actor.displayName ?? activity.actor.name}
    </a>
    {#if activity.metadata.assignee.id === activity.actor.id}
        <span>unassigned themselves.</span>
    {:else}
        <span>
            unassigned
            {activity.metadata.assignee.displayName ?? activity.metadata.assignee.name}.
        </span>
    {/if}
</div>
