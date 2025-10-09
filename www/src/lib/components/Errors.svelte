<script lang="ts">
	import type { HTMLAttributes, HTMLOlAttributes } from 'svelte/elements';

	const {
		errors,
		transforms,
		class: cls,
		...props
	}: {
		errors?: string[];
		transforms?: Record<string, string>;
	} & HTMLOlAttributes = $props();
</script>

{#if errors != null && errors.length > 0}
	<ol {...props} class="list-decimal pl-4 {cls}">
		{#each errors as error}
			<li class="pl-0">{transforms ? transforms[error] : error}</li>
		{/each}
	</ol>
{/if}
