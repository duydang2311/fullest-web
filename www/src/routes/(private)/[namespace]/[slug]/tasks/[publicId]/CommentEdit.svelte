<script lang="ts">
	import { createRef } from '@duydang2311/svutils';
	import { Editor } from '@tiptap/core';
	import { untrack } from 'svelte';
	import { createTextEditor } from '~/lib/components/editor';
	import { CheckOutline, XOutline } from '~/lib/components/icons';
	import type { Comment } from '~/lib/models/comment';
	import { editComment } from '~/lib/remotes/comment.remote';
	import { button } from '~/lib/utils/styles';

	const {
		comment,
		onCancel,
	}: { comment?: Pick<Comment, 'id' | 'contentJson'>; onCancel: () => void } = $props();
	const editor = createRef<Editor>();
</script>

<div>
	<div
		{@attach (node) => {
			untrack(() => {
				editor.current = createTextEditor({
					element: node,
					content: comment?.contentJson ? JSON.parse(comment.contentJson) : undefined,
					placeholder: 'Enter your comment...',
					editorProps: {
						attributes: {
							class: 'prose max-w-none p-4 focus:outline-none wrap-anywhere overflow-auto max-h-96',
							style: 'wrap-word: normal;',
						},
					},
					onTransaction: (props) => {
						editor.current = null!;
						editor.current = props.editor;
					},
				});

				editor.current.commands.focus();
			});
		}}
	></div>
	<div class="flex justify-end gap-2 p-2">
		<button
			type="button"
			class="{button({ variant: 'base', filled: true, outlined: true })} flex items-center gap-2"
			onclick={onCancel}
		>
			<XOutline />
			<span>Cancel</span>
		</button>
		<button
			type="button"
			class="{button({ variant: 'primary', filled: true, outlined: true })} flex items-center gap-2"
			onclick={async () => {
				if (!comment) {
					return;
				}
				const edited = await editComment({
					id: comment.id,
					contentJson: JSON.stringify(editor.current?.getJSON()),
				});
			}}
		>
			<CheckOutline />
			<span>Save</span>
		</button>
	</div>
</div>
