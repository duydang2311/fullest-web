<script lang="ts">
    import { page } from '$app/state';
    import AuthenticatedHeader from '~/lib/components/AuthenticatedHeader.svelte';
    import Footer from '~/lib/components/Footer.svelte';
    import SidebarLayout from '~/lib/components/SidebarLayout.svelte';
    import { createTabs } from '~/lib/components/builders.svelte';
    import { indexOf } from '~/lib/utils/string';
    import Tabs from './Tabs.svelte';

    const { data, children } = $props();
    const id = $props.id();
    const tabs = createTabs({
        id,
        get value() {
            const pathname = page.url.pathname;
            const idx = indexOf(pathname, '/'.charCodeAt(0), 3);
            if (idx === -1) {
                return pathname;
            }
            return pathname.substring(0, idx);
        },
    });
</script>

<SidebarLayout>
    {#snippet top()}
        <AuthenticatedHeader user={data.user} />
    {/snippet}
    {#snippet left()}
        <div class="px-4 py-2">
            <Tabs {tabs} />
        </div>
    {/snippet}
    {#snippet main()}
        <main class="bg-surface flex flex-1 flex-col">
            <div
                {...tabs.getContentProps({ value: tabs.value! })}
                class="flex w-full flex-1 flex-col"
            >
                {@render children()}
            </div>
        </main>
        <Footer />
    {/snippet}
</SidebarLayout>
