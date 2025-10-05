<script lang="ts">
	import About from './About.svelte';

	const { data } = $props();
</script>

<About project={data.project} />
