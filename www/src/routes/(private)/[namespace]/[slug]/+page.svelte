<script lang="ts">
	import { page } from '$app/state';
	import { CheckCircleOutline } from '~/lib/components/icons';
	import About from './About.svelte';
	import ProjectActions from './ProjectActions.svelte';

	const { data } = $props();
	data.project.summary = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.';
</script>

<h1 class="sr-only">
	{data.namespace.kind === 'user' ? data.namespace.user.name : ''}/{data.project.name}
</h1>
<div class="border-base-border border-b pb-4">
	{#if data.isCreatedJustNow}
		<div class="text-positive c-help-text mb-2 flex items-center gap-2">
			<CheckCircleOutline />
			<span>Project created successfully</span>
		</div>
	{/if}
	<div class="flex items-center justify-between">
		<div class="flex items-center gap-2">
			<div class="size-avatar-sm bg-base-dark rounded-full"></div>
			<a
				href="/{page.params.namespace}/{page.params.slug}"
				class="c-link text-title-lg text-title-xs font-semibold tracking-tight"
			>
				{data.project.name}
			</a>
		</div>
		<div>
			<ProjectActions />
		</div>
	</div>
	{#if data.project.summary && data.project.summary.length > 0}
		<p class="max-w-container-md text-base-fg-dim mt-2">
			{data.project.summary}
		</p>
	{/if}
</div>
<div class="mt-4">
	<About project={data.project} />
</div>
