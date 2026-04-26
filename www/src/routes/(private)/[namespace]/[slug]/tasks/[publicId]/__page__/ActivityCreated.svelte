<script lang="ts">
    import type { activityValidators } from '~/lib/components/activity-feed/utils.svelte';
    import type { ActivityKind } from '~/lib/models/activity';
    import { namespaceUrl } from '~/lib/utils/url';
    import type { InferOutput } from '~/lib/utils/validation';

    export const {
        activity,
    }: { activity: InferOutput<(typeof activityValidators)[ActivityKind.Created]> } = $props();
</script>

<div>
    <a href={namespaceUrl(activity.actor.name)} class="c-link font-medium">
        {activity.actor.displayName ?? activity.actor.name}
    </a>
    <span>created the task</span>
</div>
