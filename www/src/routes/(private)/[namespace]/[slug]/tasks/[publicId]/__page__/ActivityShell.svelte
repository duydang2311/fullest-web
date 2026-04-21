<script lang="ts">
    import { boolAttr } from 'runed';
    import type { Snippet } from 'svelte';
    import Avatar from '~/lib/components/Avatar.svelte';
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
        data-indent={boolAttr(!isCommentedActivity)}
        class="group flex items-start gap-2 data-indent:ml-3 relative data-indent:text-sm"
    >
        {#if !isCommentedActivity}
            <a href="/{activity.actor.name}" class="shrink-0">
                <Avatar user={activity.actor} class="size-avatar-xs" />
            </a>
        {/if}
        <div class="flex-1 flex justify-between items-center gap-4">
            <div class="flex-1">
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
