<script lang="ts">
	import { enhance } from '$app/forms';
	import { createRef } from '@duydang2311/svutils';
	import { Editor } from '@tiptap/core';
	import { untrack } from 'svelte';
	import invariant from 'tiny-invariant';
	import { createTextEditor } from '~/lib/components/editor';
	import { PlusOutline } from '~/lib/components/icons';
	import { button } from '~/lib/utils/styles';

	const { taskId }: { taskId: string } = $props();
	const editor = createRef<Editor>();
</script>

<form
	method="post"
	use:enhance={(e) => {
		invariant(editor.current, 'editor must not be null');
		e.formData.set('contentJson', JSON.stringify(editor.current.getJSON()));
		return (e) => {
			invariant(editor.current, 'editor must not be null');
			editor.current.commands.clearContent();
			return e.update();
		};
	}}
>
	<div class="flex gap-2">
		<img
			src="https://avatars.githubusercontent.com/u/34796192?v=4"
			class="size-avatar-sm shrink-0 rounded-full"
		/>
		<div
			class="flex-1"
			style="display: none;"
			{@attach (node) => {
				untrack(() => {
					node.style.display = '';
					editor.current = createTextEditor({
						element: node,
						placeholder: 'Add a comment...',
						editorProps: {
							attributes: {
								class: 'c-editor c-editor--outlined min-h-36 max-h-96 prose max-w-none',
							},
						},
						onTransaction: (props) => {
							editor.current = null!;
							editor.current = props.editor;
						},
					});
				});
			}}
		></div>
		<noscript class="flex-1">
			<textarea
				name="contentText"
				class="c-editor c-editor--outlined prose max-h-96 min-h-36 w-full max-w-none overflow-auto"
			></textarea>
		</noscript>
	</div>
	<div class="mt-2 flex justify-end gap-2">
		<input type="hidden" name="taskId" value={taskId} />
		<button
			disabled={editor.current != null && editor.current.isEmpty}
			type="submit"
			class="{button({ variant: 'primary', outlined: true, filled: true })} flex items-center gap-2"
		>
			<PlusOutline />
			Add comment
		</button>
	</div>
</form>
