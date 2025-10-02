<script lang="ts">
	import AuthenticatedHeader from '~/lib/components/AuthenticatedHeader.svelte';
	import Footer from '~/lib/components/Footer.svelte';
</script>

<div class="flex min-h-screen flex-col">
	<AuthenticatedHeader />
	<main class="flex-1"></main>
	<Footer />
</div>
