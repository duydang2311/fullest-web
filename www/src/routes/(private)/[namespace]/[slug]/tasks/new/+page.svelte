<script lang="ts">
	import { enhance } from '$app/forms';
	import { createTextEditor, TextEditor } from '~/lib/components/editor.svelte';
	import { PlusOutline } from '~/lib/components/icons';
	import { ErrorKind } from '~/lib/utils/errors.js';
	import { button, field, input, label } from '~/lib/utils/styles';

	const { data, form } = $props();
	let editor: TextEditor | null = null;
	const fieldErrors = $derived(form?.errors ?? {});
</script>

<form method="post" use:enhance>
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
				minlength="0"
				placeholder="Enter task title"
			/>
			{#if fieldErrors.title}
				<p class="c-help-text text-negative">{fieldErrors.title}</p>
			{/if}
		</div>
		<div class={field()}>
			<!-- svelte-ignore a11y_label_has_associated_control -->
			<!-- svelte-ignore a11y_no_noninteractive_element_interactions -->
			<label
				id="description-label"
				for="description"
				class={label()}
				onmousedown={() => {
					editor?.current.commands.focus();
				}}
				{@attach (node) => {
					node.removeAttribute('for');
				}}
			>
				Description
			</label>
			<div
				{@attach (node) => {
					editor = createTextEditor({
						element: node,
						editorProps: {
							attributes: {
								'aria-labelledby': 'description-label',
								class: `${input()} min-h-32 max-h-96 overflow-auto prose max-w-none prose-ol:pl-8 prose-ul:pl-8`,
							},
						},
						placeholder: 'Enter task description',
					});
				}}
			></div>
			<noscript class={field()}>
				<textarea
					id="description"
					title="description"
					class="{input()} h-full max-h-96 min-h-32 overflow-auto"
					placeholder="Enter task description"
				></textarea>
			</noscript>
			<p class="c-help-text">Supported text format: Plain text, Markdown.</p>
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
			{form.errors}
			<p class="c-help-text text-negative">
				You do not have permission to create tasks in this project.
			</p>
		{/if}
	{/if}
</form>
