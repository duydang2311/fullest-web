<script lang="ts">
    import type { ActivityKind } from '~/lib/models/activity';
    import { namespaceUrl } from '~/lib/utils/url';
    import type { InferOutput } from '~/lib/utils/validation';
    import type { activityValidators } from './utils.svelte';

    export const {
        activity,
    }: { activity: InferOutput<(typeof activityValidators)[ActivityKind.StatusChanged]> } =
        $props();
</script>

<div class="flex gap-2 items-center">
    <div>
        <a href={namespaceUrl(activity.actor.name)} class="c-link font-medium">
            {activity.actor.displayName ?? activity.actor.name}
        </a>
        <span>changed the status to</span>
        <span class="px-2 bg-base rounded-sm font-medium">
            {activity.metadata.status?.name ?? 'None'}
        </span>
    </div>
    <div
        class="ml-auto px-2 bg-base rounded-sm font-medium line-through opacity-40"
    >
        {activity.metadata.oldStatus?.name ?? 'None'}
    </div>
</div>
