<script lang="ts">
    import AuthenticatedHeader from '~/lib/components/AuthenticatedHeader.svelte';
    import Footer from '~/lib/components/Footer.svelte';
    import type { UserPreset } from '~/lib/models/user';

    const { user }: { user: UserPreset['Avatar'] } = $props();
</script>

<div class="flex min-h-screen flex-col">
    <AuthenticatedHeader {user} />
    <main class="flex-1"></main>
    <Footer />
</div>
