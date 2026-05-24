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
    <header class="p-4 pb-0 border-b border-surface-border text-sm">
        <div class="flex justify-between items-center gap-8">
            <nav class="flex items-center gap-2">
                <a
                    href="/"
                    aria-label="Home"
                    class={C.button({ variant: 'base', ghost: true, size: 'sm' })}
                >
                    <Logo />
                </a>
                <Breadcrumbs />
            </nav>
            <AuthenticatedHeaderPfp user={data.user} />
        </div>
        <div class="-ml-2">
            <Tabs />
        </div>
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
