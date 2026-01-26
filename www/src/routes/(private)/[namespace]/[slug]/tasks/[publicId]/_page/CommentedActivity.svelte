<script lang="ts">
    import sanitizeHtml from 'sanitize-html';
    import { renderToHTMLString } from '~/lib/components/editor';
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { User } from '~/lib/models/user';
    import { v } from '~/lib/utils/valibot';
    import { createValidator } from '~/lib/utils/validation';
    import ActivityAvatar from './ActivityAvatar.svelte';
    import CommentActions from './CommentActions.svelte';
    import CommentEdit from './CommentEdit.svelte';

    const {
        taskId,
        activity,
    }: {
        taskId: string;
        activity: Pick<Activity, 'createdTime' | 'data'> & {
            actor: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
        };
    } = $props();

    const validator = createValidator(
        v.object({
            comment: v.object({
                id: v.string(),
                contentJson: v.optional(v.string()),
            }),
        })
    );
    const validation = $derived(validator.parse(activity.data));
    const comment = $derived(validation.ok ? validation.data.comment : null);
    let isEditing = $state.raw(false);
</script>

{#if comment}
    <div class="border-base-border flex-1 overflow-auto rounded-md border">
        <div
            class="bg-base-dark border-b border-b-base-border flex items-center gap-nbsp rounded-t-md px-2 py-2"
        >
            <ActivityAvatar user={activity.actor} />
            <span>
                <span class="text-sm font-medium text-base-fg-dim">
                    · <RelativeDateTime time={activity.createdTime} />
                </span>
            </span>
            <div class="ml-auto">
                <CommentActions {taskId} {comment} bind:isEditing />
            </div>
        </div>
        {#if isEditing}
            <CommentEdit {taskId} {comment} bind:isEditing />
        {:else}
            <article class="prose max-w-none p-4 wrap-anywhere">
                {#if comment.contentJson && comment?.contentJson.length > 0}
                    {@html sanitizeHtml(renderToHTMLString(JSON.parse(comment.contentJson)))}
                {:else}
                    <i class="text-base-fg-muted">No description provided.</i>
                {/if}
            </article>
        {/if}
    </div>
{/if}
