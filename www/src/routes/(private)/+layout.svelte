<script lang="ts">
	import AuthenticatedHeader from '$lib/components/AuthenticatedHeader.svelte';
	import Footer from '$lib/components/Footer.svelte';

	const { children } = $props();
</script>

<div class="flex min-h-screen flex-col">
	<AuthenticatedHeader />
	{@render children()}
	<Footer />
</div>
