<script lang="ts">
    import AuthenticatedHeaderPfp from '~/lib/components/AuthenticatedHeaderPfp.svelte';
    import Footer from '~/lib/components/Footer.svelte';
    import { Logo } from '~/lib/components/icons';
    import { C } from '~/lib/utils/styles';
    import Breadcrumbs from './__page__/Breadcrumbs.svelte';
    import Tabs from './__page__/Tabs.svelte';

    const { data, children } = $props();
</script>

<div class="flex flex-col min-h-screen">
    <header class="border-b border-surface-border p-4 pb-0 text-sm">
        <nav class="flex items-center justify-between gap-8">
            <div class="flex items-center gap-2 -ml-2">
                <a
                    href="/"
                    aria-label="Home"
                    class={C.button({ variant: 'base', ghost: true, size: 'sm' })}
                >
                    <Logo />
                </a>
                <Breadcrumbs />
            </div>
            <div class="flex items-center gap-2">
                <AuthenticatedHeaderPfp user={data.user} />
            </div>
        </nav>
        <nav class="-ml-4">
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
