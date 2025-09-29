<script lang="ts">
	import Anonymous from './Anonymous.svelte';
	import Authenticated from './Authenticated.svelte';

	const { data } = $props();
</script>

{#if data.session}
	<Authenticated />
{:else}
	<Anonymous />
{/if}
