<script lang="ts">
	import { CheckCircleOutline } from '~/lib/components/icons';
	import ProjectActions from './ProjectActions.svelte';

	const { data } = $props();
</script>

<main class="bg-base-light flex-1 p-8">
	<h1 class="sr-only">
		{data.namespace.kind === 'user' ? data.namespace.user.name : ''}/{data.project.name}
	</h1>
	<div class="c-main max-w-container-xl mx-auto">
		{#if data.isCreatedJustNow}
			<div class="text-positive c-help-text mb-8 flex items-center gap-2">
				<CheckCircleOutline />
				<span>Project created successfully</span>
			</div>
		{/if}
		<div class="flex justify-between">
			<div class="text-title-sm flex items-center gap-1">
				<a href="/{data.namespace.user.name}" class="c-link not-hover:text-base-fg-muted">
					{data.namespace.kind === 'user' ? data.namespace.user.name : ''}
				</a>
				<span class="text-base-fg-muted">/</span>
				<a
					href="/{data.namespace.user.name}/{data.project.identifier}"
					class="c-link not-hover:text-base-fg-strong font-bold">{data.project.name}</a
				>
			</div>
			<div class="hidden md:block">
				<ProjectActions />
			</div>
		</div>
		{#if data.project.summary && data.project.summary.length > 0}
			<p class="max-w-container-md text-base-fg-dim mt-2">
				{data.project.summary}
			</p>
		{/if}
		<div class="mt-4 md:hidden">
			<ProjectActions />
		</div>
	</div>
</main>
