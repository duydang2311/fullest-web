<script lang="ts">
    import sanitizeHtml from 'sanitize-html';
    import Avatar from '~/lib/components/Avatar.svelte';
    import { renderToHTMLString } from '~/lib/components/editor';
    import type { Comment } from '~/lib/models/comment';
    import type { Task } from '~/lib/models/task';
    import type { User } from '~/lib/models/user';
    import Stats from './Stats.svelte';

    const {
        task,
        comment,
    }: {
        task: Pick<Task, 'title' | 'publicId'> & { author: Pick<User, 'name' | 'displayName'> };
        comment: Pick<Comment, 'id' | 'contentJson'> & {
            author: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
        };
    } = $props();
    let isEditing = $state.raw(false);
</script>

<div>
    <h1>
        <span class="text-title-sm text-base-fg-strong font-bold">{task.title}</span>
        <span class="c-help-text inline-block align-text-top">#{task.publicId}</span>
    </h1>
    <div class="flex gap-2 items-center mt-2">
        <Avatar user={comment.author} class="size-avatar-xs shrink-0 rounded-full" />
        <div>
            <strong class="text-base-fg-strong">
                {task.author.displayName ?? task.author.name}
            </strong> opened this task 2 days ago.
        </div>
    </div>
    <div class="mt-4 flex flex-wrap gap-x-8 gap-y-2 *:min-w-40 lg:hidden">
        <Stats />
    </div>
    <hr class="border-base-border-weak mt-4 w-full" />
    <article class="prose max-w-none wrap-anywhere mt-4">
        {#if comment?.contentJson && comment?.contentJson.length > 0}
            {@html sanitizeHtml(renderToHTMLString(JSON.parse(comment.contentJson)))}
        {:else}
            <i class="text-base-fg-muted">No description provided.</i>
        {/if}
    </article>
</div>
