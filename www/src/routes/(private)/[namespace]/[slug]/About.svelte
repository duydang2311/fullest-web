<script lang="ts">
	import type { Project } from '~/lib/models/project';

	interface Props {
		project: Pick<Project, 'about' | 'tags'>;
	}
	const { project }: Props = $props();
</script>

<h2 class="text-fg-emph tracking-tight">About</h2>
{#if project.about && project.about.length > 0}
	<p class="mt-4">{project.about}</p>
{/if}
