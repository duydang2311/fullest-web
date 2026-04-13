<script lang="ts">
    import invariant from 'tiny-invariant';
    import AuthenticatedHeader from '~/lib/components/AuthenticatedHeader.svelte';
    import Footer from '~/lib/components/Footer.svelte';
    import SidebarLayout from '~/lib/components/SidebarLayout.svelte';
    import { usePageData } from '~/lib/utils/kit';
    import type { PageData } from './$types';
    import RecentActivities from './RecentActivities.svelte';
    import YourProjects from './YourProjects.svelte';

    const data = usePageData<PageData>();
    invariant(data.session, 'data.session must not be null');
    const user = $derived(data.session.user);
</script>

<SidebarLayout>
    {#snippet top()}
        <AuthenticatedHeader {user} />
    {/snippet}
    {#snippet left()}
        <YourProjects />
    {/snippet}
    {#snippet main()}
        <main class="p-6 flex-1">
            <h1 class="text-title-sm text-fg-emph">
                Good morning, {user.displayName ?? user.name}!
            </h1>
            <div class="mt-6">
                <RecentActivities />
            </div>
        </main>
        <Footer />
    {/snippet}
</SidebarLayout>
