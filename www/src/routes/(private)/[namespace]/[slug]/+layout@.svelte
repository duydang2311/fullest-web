<script lang="ts">
    import { goto, onNavigate } from '$app/navigation';
    import { page } from '$app/state';
    import { Flip } from 'gsap/dist/Flip';
    import AuthenticatedHeaderCreateActions from '~/lib/components/AuthenticatedHeaderCreateActions.svelte';
    import AuthenticatedHeaderPfp from '~/lib/components/AuthenticatedHeaderPfp.svelte';
    import Footer from '~/lib/components/Footer.svelte';
    import { createTabs } from '~/lib/components/builders.svelte';
    import { ChevronRightOutline } from '~/lib/components/icons';
    import { gsap } from '~/lib/utils/gsap';
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
            await goto(details.value, { keepFocus: true });
        },
    });

    gsap.registerPlugin(Flip);

    onNavigate((e) => {
        if (e.willUnload) {
            return;
        }

        const state = Flip.getState('#active-tab-underline');
        e.complete.then(() => {
            Flip.from(state, {
                targets: '#active-tab-underline',
                duration: 0.3,
                ease: 'circ.inOut',
            });
        });
    });
</script>

<div class="flex min-h-screen flex-col">
    <header class="text-sm bg-surface-subtle">
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
    </header>
    <main class="bg-surface flex flex-1 flex-col">
        <!-- <div
			{...tabs.getContentProps({ value: tabs.value! })}
			class="max-w-container-xl mx-auto flex w-full flex-1 flex-col"
		>
			{@render children()}
		</div> -->
        <div {...tabs.getContentProps({ value: tabs.value! })} class="flex w-full flex-1 flex-col">
            {@render children()}
        </div>
    </main>
    <Footer />
</div>
