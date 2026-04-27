<script lang="ts">
    import { boolAttr } from 'runed';
    import type { Snippet } from 'svelte';
    import Avatar from '~/lib/components/Avatar.svelte';
    import {
        IconChatDotsOutline
    } from '~/lib/components/icons';
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import { ActivityKind, type Activity } from '~/lib/models/activity';
    import type { UserPreset } from '~/lib/models/user';

    const {
        activity,
        children,
    }: {
        activity: Pick<Activity, 'createdTime' | 'kind'> & {
            actor: UserPreset['Avatar'];
        };
        children: Snippet;
    } = $props();
    const isCommentedActivity = $derived(activity.kind === ActivityKind.Commented);
</script>

<div>
    <div
        class="absolute h-[calc(100%+1.5rem)] w-px bg-surface-border left-[calc(var(--spacing-avatar-xs)/2)] top-0"
    ></div>
    <div
        data-comment={boolAttr(isCommentedActivity)}
        class="group flex items-start gap-4 relative not-data-comment:text-sm data-comment:mt-3.5"
    >
        {#if isCommentedActivity}
            <div
                class="size-avatar-xs shrink-0 bg-primary text-primary-fg rounded-full p-px outline-4 outline-primary"
            >
                <IconChatDotsOutline class="size-full" />
            </div>
        {:else}
            <a href="/{activity.actor.name}" class="shrink-0 relative">
                <Avatar user={activity.actor} class="size-avatar-xs" />
            </a>
        {/if}
        <div class="flex-1 flex justify-between items-center gap-4">
            <div class="flex-1 group-data-comment:-mt-3.5">
                {@render children()}
            </div>
            {#if !isCommentedActivity}
                <div>
                    <div class="c-help-text text-fg-muted">
                        <span><RelativeDateTime time={activity.createdTime} /></span>
                    </div>
                </div>
            {/if}
        </div>
    </div>
</div>
