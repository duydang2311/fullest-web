<script lang="ts">
	import type { Comment } from '~/lib/models/comment';
	import type { User } from '~/lib/models/user';
	import AddComment from './AddComment.svelte';
	import CommentItem from './CommentItem.svelte';

	const {
		taskId,
		comments,
	}: {
		taskId: string;
		comments: (Pick<Comment, 'id' | 'contentJson' | 'createdTime'> & {
			author: Pick<User, 'name'>;
		})[];
	} = $props();
</script>

<section>
	<h2 class="text-title-xs text-base-fg-strong font-semibold tracking-tight">Comments</h2>
	{#if comments.length > 0}
		<ol class="mt-4 flex flex-col gap-4">
			{#each comments as comment (comment.id)}
				<li>
					<CommentItem {comment} />
				</li>
			{/each}
		</ol>
	{/if}
	<div class="mt-8">
		<AddComment {taskId} />
	</div>
</section>
