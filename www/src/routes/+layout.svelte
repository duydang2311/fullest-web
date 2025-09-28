<script lang="ts">
	import '../app.css';
	import '@fontsource-variable/manrope';
	import '@fontsource-variable/geist-mono';
	import favicon from '$lib/assets/favicon.svg';

	const { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

{@render children?.()}
