<script lang="ts">
    import type { ActivityKind } from '~/lib/models/activity';
    import { namespaceUrl } from '~/lib/utils/url';
    import type { InferOutput } from '~/lib/utils/validation';
    import type { activityValidators } from './utils.svelte';

    export const {
        activity,
    }: { activity: InferOutput<(typeof activityValidators)[ActivityKind.TitleChanged]> } = $props();
</script>

<div>
    <a href={namespaceUrl(activity.actor.name)} class="c-link font-medium text-fg">
        {activity.actor.displayName ?? activity.actor.name}
    </a>
    <span>
        changed the title to
        <span class="outline-4 outline-base ml-2 bg-base rounded-sm font-medium text-fg">
            {activity.metadata.title}
        </span>
    </span>
</div>
