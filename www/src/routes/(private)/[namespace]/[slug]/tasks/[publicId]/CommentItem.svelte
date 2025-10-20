<script lang="ts">
import sanitizeHtml from 'sanitize-html';
	import { renderToHTMLString } from '~/lib/components/editor';
	import type { Comment } from '~/lib/models/comment';
	import CommentActions from './CommentActions.svelte';
	import CommentEdit from './CommentEdit.svelte';
	import type { User } from '~/lib/models/user';

	const {
		comment,
	}: {
		comment: Pick<Comment, 'id' | 'contentJson'> & {
			author: Pick<User, 'name'>;
		};
	} = $props();
	let isEditing = $state.raw(false);
</script>

<div class="flex gap-2">
	<img
		src="https://avatars.githubusercontent.com/u/34796192?v=4"
		class="size-avatar-sm shrink-0 rounded-full max-lg:hidden"
	/>
	<div class="border-base-border flex-1 overflow-auto rounded-lg border">
		<div
			class="border-b-base-border bg-base-dark flex items-center gap-2 rounded-t-lg border-b px-4 py-2"
		>
			<img
				src="https://avatars.githubusercontent.com/u/34796192?v=4"
				class="size-avatar-sm rounded-full lg:hidden"
			/>
			<span
				><strong class="text-base-fg-strong">{comment.author.name}</strong> created 2 days ago</span
			>
			<div class="ml-auto">
				<CommentActions {comment} bind:isEditing />
			</div>
		</div>
		{#if isEditing}
			<CommentEdit
				{comment}
				onCancel={() => {
					isEditing = false;
				}}
			/>
		{:else}
			<article class="prose max-w-none p-4">
				{#if comment?.contentJson && comment?.contentJson.length > 0}
					{@html sanitizeHtml(renderToHTMLString(JSON.parse(comment.contentJson)))}
				{:else}
					<i class="text-base-fg-muted">No description provided.</i>
				{/if}
			</article>
		{/if}
	</div>
</div>
