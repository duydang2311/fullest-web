<script lang="ts">
    import invariant from 'tiny-invariant';
    import AuthenticatedHeader from '~/lib/components/AuthenticatedHeader.svelte';
    import Footer from '~/lib/components/Footer.svelte';
    import { usePageData } from '~/lib/utils/kit';
    import type { PageData } from './$types';
    import RecentActivities from './RecentActivities.svelte';
    import YourProjects from './YourProjects.svelte';

    const data = usePageData<PageData>();
    invariant(data.session, 'data.session must not be null');
    const user = $derived(data.session.user);
</script>

<div class="bg-surface-subtle flex flex-col h-screen overflow-hidden">
    <AuthenticatedHeader {user} />
    <div class="flex flex-1 overflow-hidden">
        <YourProjects />
        <main
            class="flex-1 bg-surface border-t border-l border-surface-border rounded-tl-2xl flex flex-col overflow-auto"
        >
            <div class="p-6 flex-1">
                <h1 class="text-title-sm text-fg-emph">
                    Good morning, {user.displayName ?? user.name}!
                </h1>
                <div class="mt-6">
                    <RecentActivities />
                </div>
            </div>
            <Footer />
        </main>
    </div>
</div>
