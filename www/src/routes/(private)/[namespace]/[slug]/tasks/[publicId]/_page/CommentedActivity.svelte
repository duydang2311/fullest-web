<script lang="ts">
    import { renderToHTMLString } from '~/lib/components/editor';
    import RelativeDateTime from '~/lib/components/RelativeDateTime.svelte';
    import type { Activity } from '~/lib/models/activity';
    import type { User } from '~/lib/models/user';
    import { usePageData } from '~/lib/utils/kit';
    import { namespaceUrl } from '~/lib/utils/url';
    import type { PageData } from '../$types';
    import ActivityAvatar from './ActivityAvatar.svelte';
    import CommentActions from './CommentActions.svelte';
    import CommentEdit from './CommentEdit.svelte';

    const {
        activity,
    }: {
        activity: Pick<Activity, 'createdTime'> & {
            actor: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
            metadata: { comment: { id: string; contentJson: string | null } };
        };
    } = $props();

    const taskId = $derived(usePageData<PageData>().task.id);
    const comment = $derived(activity.metadata.comment);
    let isEditing = $state.raw(false);
</script>

{#if comment}
    <div class="border-base-border flex-1 overflow-auto rounded-md border">
        <div
            class="bg-surface-subtle border-b border-b-base-border flex items-center gap-2 rounded-t-md px-2 py-2"
        >
            <ActivityAvatar user={activity.actor} />
            <span>
                <a href={namespaceUrl(activity.actor.name)} class="c-link">
                    <strong>{activity.actor.displayName ?? activity.actor.name}</strong>
                </a>
                <span class="text-sm font-medium text-fg-dim">
                    · <RelativeDateTime time={activity.createdTime} />
                </span>
            </span>
            <div class="ml-auto">
                <CommentActions {comment} bind:isEditing />
            </div>
        </div>
        {#if isEditing}
            <CommentEdit {taskId} {comment} bind:isEditing />
        {:else}
            <article class="prose max-w-none p-4 wrap-anywhere">
                {#if comment.contentJson && comment?.contentJson.length > 0}
                    <!-- eslint-disable-next-line svelte/no-at-html-tags -->
                    {@html renderToHTMLString(JSON.parse(comment.contentJson))}
                {:else}
                    <i class="text-fg-muted">No description provided.</i>
                {/if}
            </article>
        {/if}
    </div>
{/if}
