<script lang="ts">
    import type { Snippet } from 'svelte';
    import Avatar from '~/lib/components/Avatar.svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { UserPreset } from '~/lib/models/user';
    import RelativeDateTime from '../RelativeDateTime.svelte';
    import { activityValidators } from './utils.svelte';

    const {
        activity,
        children,
    }: {
        activity: Pick<Activity, 'createdTime'> & {
            actor: UserPreset['Avatar'];
        };
        children: Snippet;
    } = $props();

    const projectParsed = $derived(activityValidators.projectContext.parse(activity));
    const taskParsed = $derived(activityValidators.taskContext.parse(activity));
</script>

<div class="relative">
    <div class="flex items-start gap-2">
        <a href="/{activity.actor.name}" class="shrink-0">
            <Avatar user={activity.actor} class="size-avatar-sm" />
        </a>
        <div class="flex-1">
            <div>
                {@render children()}
            </div>
            <div>
                <div class="c-help-text text-fg-muted">
                    <span><RelativeDateTime time={activity.createdTime} /></span>
                    {#if projectParsed.ok && taskParsed.ok && projectParsed.data.project.namespace.user}
                        <span>·</span>
                        <span>in</span>
                        <a
                            href="/{projectParsed.data.project.namespace.user.name}/{projectParsed
                                .data.project.identifier}"
                            class="c-link"
                        >
                            {projectParsed.data.project.name}
                        </a>
                        <span>·</span>
                        <a
                            href="/{projectParsed.data.project.namespace.user.name}/{projectParsed
                                .data.project.identifier}/tasks/{taskParsed.data.task.publicId}"
                            class="c-link"
                        >
                            {taskParsed.data.task.title}
                        </a>
                    {:else if projectParsed.ok && projectParsed.data.project.namespace.user}
                        <span>·</span>
                        <span>in</span>
                        <a
                            href="/{projectParsed.data.project.namespace.user.name}/{projectParsed
                                .data.project.identifier}"
                            class="c-link"
                        >
                            {projectParsed.data.project.name}
                        </a>
                    {/if}
                </div>
            </div>
        </div>
    </div>
    <div
        class="group-last:hidden absolute bg-surface-border h-full w-px top-(--spacing-avatar-sm) left-[calc(var(--spacing-avatar-sm)/2)]"
    ></div>
</div>
