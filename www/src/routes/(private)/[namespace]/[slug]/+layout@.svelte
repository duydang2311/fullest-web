<script lang="ts">
    import { goto } from '$app/navigation';
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
            const idx = indexOf(page.url.pathname, '/'.charCodeAt(0), 3);
            if (idx === -1) {
                return page.url.pathname;
            }
            return page.url.pathname.substring(0, idx);
        },
        onValueChange: async (details) => {
            await goto(details.value, { replaceState: true, keepFocus: true });
        },
    });
</script>

<SidebarLayout>
    {#snippet top()}
        <!-- <header class="text-sm bg-surface-subtle">
            <nav>
                <div
                    class="flex divide-x divide-surface-border overflow-auto border-b border-b-surface-border"
                >
                    <div class="flex items-center gap-4 px-8">
                        <div class="flex items-center gap-2">
                            <a href="/{data.namespace.user.name}" class="c-link">
                                {data.namespace.kind === 'user'
                                    ? (data.namespace.user.displayName ?? data.namespace.user.name)
                                    : 'Organization'}
                            </a>
                            <ChevronRightOutline class="text-fg-muted size-4" />
                            <a
                                href="/{data.namespace.user.name}/{data.project.identifier}"
                                class="c-link font-normal"
                            >
                                {data.project.name}
                            </a>
                        </div>
                    </div>
                    <div class="flex-1">
                        <Tabs {tabs} />
                    </div>
                    <div class="flex items-center justify-end gap-4 px-8 shrink-0">
                        <AuthenticatedHeaderCreateActions />
                        <AuthenticatedHeaderPfp user={data.user} />
                    </div>
                </div>
            </nav>
        </header> -->
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
