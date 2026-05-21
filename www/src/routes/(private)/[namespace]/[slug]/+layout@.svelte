<script lang="ts">
    import { page } from '$app/state';
    import AuthenticatedHeader from '~/lib/components/AuthenticatedHeader.svelte';
    import Footer from '~/lib/components/Footer.svelte';
    import Breadcrumbs from './__page__/Breadcrumbs.svelte';
    import ProjectActions from './__page__/ProjectActions.svelte';
    import Tabs from './__page__/Tabs.svelte';
    import { useProject } from './__page__/utils.svelte';
    import AuthenticatedHeaderNav from '~/lib/components/AuthenticatedHeaderNav.svelte';

    const { data, children } = $props();
    const id = $props.id();
    const project = $derived(await useProject());
</script>

<div class="flex flex-col min-h-screen">
    <header class="border-b border-surface-border p-4 pb-0 text-sm">
        <AuthenticatedHeaderNav user={data.user} />
        <nav class="-mx-4 mt-0">
            <Tabs />
        </nav>
    </header>
    <main class="flex flex-1 flex-col">
        <div class="p-4 md:px-8 flex-1 flex flex-col">
            <div class="flex-1 flex flex-col">
                {@render children()}
            </div>
        </div>
    </main>
    <Footer />
</div>
<!-- <SidebarLayout>
    {#snippet top()}
    {/snippet}
    {#snippet left()}
        <div class="px-4 py-2">
            <Tabs {tabs} />
        </div>
    {/snippet}
    {#snippet main()}
    {/snippet}
</SidebarLayout> -->
