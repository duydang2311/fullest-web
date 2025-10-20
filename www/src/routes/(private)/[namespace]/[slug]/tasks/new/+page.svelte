<script lang="ts">
	import { enhance } from '$app/forms';
	import { createRef } from '@duydang2311/svutils';
	import type { Editor } from '@tiptap/core';
	import { createTextEditor } from '~/lib/components/editor';
	import Errors from '~/lib/components/Errors.svelte';
	import { PlusOutline } from '~/lib/components/icons';
	import InlineAlert from '~/lib/components/InlineAlert.svelte';
	import { matchNone } from '~/lib/utils/comparer.js';
	import { ErrorKind, flattenErrorEntries } from '~/lib/utils/errors.js';
	import { button, field, input, label } from '~/lib/utils/styles';

	const { data, form } = $props();
	const editor = createRef<Editor>();
	const fieldErrors = $derived(form?.errors ?? {});
</script>

<div class="max-w-container-xl mx-auto w-full px-8 py-4">
	<form
		method="post"
		use:enhance={(e) => {
			if (editor.current == null || editor.current.isEmpty) {
				return;
			}

			const json = editor.current.getJSON();
			e.formData.set('description_json', JSON.stringify(json));
		}}
	>
		<h1 class="text-title-xs font-semibold tracking-tight">Create new task</h1>
		<p class="text-base-fg-dim c-help-text italic">
			Required fields are marked with an asterisk (*).
		</p>
		<fieldset class="mt-4 space-y-4">
			<input type="hidden" name="projectId" value={data.project.id} />
			<div class={field()}>
				<label for="title" class={label()}>Title *</label>
				<input
					id="title"
					name="title"
					type="text"
					class={input()}
					required
					minlength="1"
					placeholder="Enter task title"
					aria-invalid={fieldErrors.title ? true : undefined}
				/>
				<Errors
					errors={fieldErrors.title}
					transforms={{
						required: 'Enter a title for the task.',
						string: 'Enter a title for the task.',
					}}
					class="c-help-text text-negative"
				/>
			</div>
			<div class={field()}>
				<!-- svelte-ignore a11y_label_has_associated_control -->
				<!-- svelte-ignore a11y_no_noninteractive_element_interactions -->
				<label
					id="description-label"
					for="description"
					class={label()}
					onmousedown={() => {
						editor.current?.commands.focus();
					}}
					{@attach (node) => {
						node.removeAttribute('for');
					}}
				>
					Description
				</label>
				<div
					{@attach (node) => {
						editor.current = createTextEditor({
							element: node,
							editorProps: {
								attributes: {
									'aria-labelledby': 'description-label',
									class: `${input()} min-h-32 max-h-96 overflow-auto prose max-w-none prose-ol:pl-8 prose-ul:pl-8`,
								},
							},
							placeholder: 'Enter task description',
							onTransaction: (props) => {
								editor.current = props.editor;
							},
						});
					}}
				></div>
				<noscript class={field()}>
					<textarea
						id="description"
						name="description_text"
						title="description"
						class="{input()} h-full max-h-96 min-h-32 overflow-auto"
						placeholder="Enter task description"
					></textarea>
				</noscript>
				<p class="c-help-text">Supported formats: Plain text, Markdown.</p>
			</div>
			<button
				type="submit"
				class="{button({
					variant: 'primary',
					filled: true,
					outlined: true,
				})} ml-auto flex items-center gap-2"
			>
				<PlusOutline />
				<span>Create Task</span>
			</button>
		</fieldset>
		{#if form}
			{#if form.kind === ErrorKind.Forbidden}
				<p class="c-help-text text-negative">
					You do not have permission to create tasks in this project.
				</p>
			{:else if form.kind === ErrorKind.Validation}
				{@const errors = Object.entries(form.errors)
					.filter(([k]) => matchNone('title', 'description')(k))
					.flatMap(flattenErrorEntries)}
				<InlineAlert variant="negative" header="Invalid submission">
					<p class="c-help-text">Please review and correct the following errors:</p>
					<Errors {errors} class="mt-1" />
				</InlineAlert>
			{/if}
		{/if}
	</form>
</div>
