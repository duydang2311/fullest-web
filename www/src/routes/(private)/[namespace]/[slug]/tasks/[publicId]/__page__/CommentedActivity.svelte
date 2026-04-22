<script lang="ts">
    import DOMPurify from 'dompurify';
    import { renderToHTMLString } from '~/lib/components/editor.svelte';
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import type { ActivityKind } from '~/lib/models/activity';
    import { namespaceUrl } from '~/lib/utils/url';
    import type { InferOutput } from '~/lib/utils/validation';
    import ActivityAvatar from './ActivityAvatar.svelte';
    import CommentActions from './CommentActions.svelte';
    import CommentEdit from './CommentEdit.svelte';
    import { useTask, validators } from './utils.svelte';

    const {
        activity,
    }: {
        activity: InferOutput<(typeof validators)[ActivityKind.Commented]>;
    } = $props();

    const task = $derived(useTask());
    const comment = $derived(activity.metadata.comment);
    let isEditing = $state.raw(false);
</script>

<div class="bg-surface border border-surface-border flex-1 px-3 py-2 overflow-auto rounded-md">
    <div class="flex items-center gap-2 rounded-m mb-2">
        <ActivityAvatar user={activity.actor} />
        <span>
            <a href={namespaceUrl(activity.actor.name)} class="c-link font-medium">
                {activity.actor.displayName ?? activity.actor.name}
            </a>
            <span class="text-sm text-fg-muted">
                · <RelativeDateTime time={activity.createdTime} />
            </span>
        </span>
        <div class="ml-auto">
            <CommentActions {comment} bind:isEditing />
        </div>
    </div>
    {#if isEditing}
        {#if task.current}
            <CommentEdit taskId={task.current.id} {comment} bind:isEditing />
        {/if}
    {:else}
        <article class="prose max-w-none wrap-anywhere">
            {#if comment.contentJson && comment?.contentJson.length > 0}
                {@html DOMPurify.sanitize(renderToHTMLString(JSON.parse(comment.contentJson)))}
            {:else}
                <span class="text-fg-muted text-sm">N/A.</span>
            {/if}
        </article>
    {/if}
</div>
