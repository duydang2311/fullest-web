<script lang="ts">
    import type { ActivityKind } from '~/lib/models/activity';
    import { namespaceUrl } from '~/lib/utils/url';
    import type { InferOutput } from '~/lib/utils/validation';
    import type { activityValidators } from './utils.svelte';

    export const {
        activity,
    }: { activity: InferOutput<(typeof activityValidators)[ActivityKind.Commented]> } = $props();
</script>

<div>
    <a href={namespaceUrl(activity.actor.name)} class="c-link font-medium">
        {activity.actor.displayName ?? activity.actor.name}
    </a>
    <span>left a comment</span>
    <!-- <a
        href="/{activity.project.namespace.user.name}/{activity.project.identifier}/tasks/{activity
            .task.publicId}"
        class="c-link"
    >
        {activity.task.title}
    </a>. -->
</div>
