<script lang="ts">
    import AuthenticatedHeader from '$lib/components/AuthenticatedHeader.svelte';
    import Footer from '$lib/components/Footer.svelte';

    const { children, data } = $props();
</script>

<div class="flex min-h-screen flex-col">
    <AuthenticatedHeader user={data.user} />
    {@render children()}
    <Footer />
</div>
