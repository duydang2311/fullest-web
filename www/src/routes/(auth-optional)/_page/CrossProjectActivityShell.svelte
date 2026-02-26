<script lang="ts">
    import type { Snippet } from 'svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { UserPreset } from '~/lib/models/user';
    import { formatRelativeDateTime } from '~/lib/utils/date';
    import ActivityShell from './ActivityShell.svelte';
    import { activityValidators } from './utils.svelte';

    const {
        activity,
        children,
    }: {
        activity: Pick<Activity, 'createdTime' | 'kind' | 'metadata'> & {
            actor: UserPreset['Avatar'];
        };
        children: Snippet;
    } = $props();
</script>

<ActivityShell {activity}>
    <div>
        {@render children()}
    </div>
    <div>
        <div class="c-help-text text-fg-muted">
            <span>{formatRelativeDateTime(activity.createdTime)}</span>
            {#if activityValidators.projectContext.check(activity) && activityValidators.taskContext.check(activity) && activity.project.namespace.user}
                <span>·</span>
                <span>in</span>
                <a
                    href="/{activity.project.namespace.user.name}/{activity.project.identifier}"
                    class="c-link"
                >
                    {activity.project.name}
                </a>
                <span>·</span>
                <a
                    href="/{activity.project.namespace.user.name}/{activity.project
                        .identifier}/tasks/{activity.task.publicId}"
                    class="c-link"
                >
                    {activity.task.title}
                </a>
            {:else if activityValidators.projectContext.check(activity) && activity.project.namespace.user}
                <span>·</span>
                <span>in</span>
                <a
                    href="/{activity.project.namespace.user.name}/{activity.project.identifier}"
                    class="c-link"
                >
                    {activity.project.name}
                </a>
            {/if}
        </div>
    </div>
</ActivityShell>
