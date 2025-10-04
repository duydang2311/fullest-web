<script lang="ts">
	import type { Project } from '~/lib/models/project';

	interface Props {
		project: Pick<Project, 'about' | 'tags'>;
	}
	const { project }: Props = $props();
</script>

<div>
	{#if project.about && project.about.length > 0}
		<p>{project.about}</p>
	{/if}
</div>
