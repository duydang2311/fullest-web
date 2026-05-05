<script lang="ts">
    import Footer from '~/lib/components/Footer.svelte';
    import LogoType from '~/lib/components/LogoType.svelte';
    import { button, C } from '~/lib/utils/styles';
    import FeaturesSection from './FeaturesSection.svelte';
    import HeaderActions from './HeaderActions.svelte';
    import ScrollingText from './ScrollingText.svelte';
</script>

<header class="mb-8 p-4 fade-up" style:--_fade-up-delay="0ms">
    <div
        class="bg-surface dark:bg-base-subtle border-base-border sticky top-0 h-16 content-center rounded-full border px-8"
    >
        <nav class="flex items-center justify-between">
            <LogoType />
            <HeaderActions />
        </nav>
    </div>
</header>
<main class="relative">
    <section class="min-h-[calc(100vh-2rem-4rem-2rem)] p-8">
        <div class="text-center fade-up mt-16" style="--_fade-up-delay: 30ms;">
            <h1
                class="text-fg-emph font-medium text-[clamp(var(--text-display-md),calc(var(--text-body-md)+6vw),6rem)] text-balance tracking-tighter"
            >
                <ScrollingText />
            </h1>
            <p
                class="mt-16 font-medium fade-up-slow max-w-lg mx-auto text-pretty text-fg-dim"
                style="--_fade-up-delay: 30ms;"
            >
                Work openly, build accountability, and create a portfolio of progress that reflects
                every step you take.
            </p>
        </div>
        <div class="mt-4 fade-up-slow" style="--_fade-up-delay: 120ms;">
            <div class="mt-2 flex justify-center">
                <a
                    href="/sign-in"
                    class="{button({
                        variant: 'primary',
                        filled: true,
                    })} px-4"
                >
                    Get started
                </a>
            </div>
        </div>
        <div class="hero-visual mt-12 pb-16">
            <div
                class="border border-surface-border rounded-xl overflow-hidden bg-surface-subtle max-w-container-lg mx-auto fade-up-slow"
                style="--_fade-up-delay: 300ms;"
            >
                <div class="flex items-center gap-2 p-4">
                    <span class="size-2.5 rounded-full bg-red-500"></span>
                    <span class="size-2.5 rounded-full bg-yellow-500"></span>
                    <span class="size-2.5 rounded-full bg-green-500"></span>
                    <span class="text-sm text-fg-muted ml-1">Website Redesign</span>
                </div>
                <div class="flex relative">
                    <div
                        class="p-4 absolute sm:static sm:translate-x-0 sm:opacity-100 bg-surface h-full border-t border-r sm:border-r-0 sm:border-t-0 sm:bg-transparent border-surface-border -translate-x-[95%] opacity-0 hover:opacity-100 hover:translate-x-0 transition duration-200"
                    >
                        <div class="text-fg-muted text-sm">Projects</div>
                        <button
                            type="button"
                            class="flex items-center gap-4 px-2 py-1 bg-base-subtle rounded-lg mt-1 w-full"
                        >
                            <span class="size-2 rounded-full bg-violet-500"></span>
                            <span>Website Redesign</span>
                        </button>
                        <button
                            type="button"
                            class="flex items-center gap-4 px-2 py-1 hover:bg-base-subtle rounded-lg mt-1 w-full"
                        >
                            <span class="size-2 rounded-full bg-amber-500"></span>
                            <span>API v2</span>
                        </button>
                        <button
                            type="button"
                            class="flex items-center gap-4 px-2 py-1 hover:bg-base-subtle rounded-lg mt-1 w-full"
                        >
                            <span class="size-2 rounded-full bg-sky-500"></span>
                            <span>Mobile App</span>
                        </button>
                    </div>
                    <div
                        class="p-4 bg-surface border-t sm:border-l sm:rounded-tl-xl border-surface-border flex-1"
                    >
                        <div class="flex justify-between items-center gap-8">
                            <span class="text-fg-emph font-semibold">All tasks</span>
                            <div class="flex gap-2">
                                <button class={C.button({ filled: true, size: 'sm' })}>
                                    Filter
                                </button>
                                <button
                                    class="{C.button({
                                        filled: true,
                                        size: 'sm',
                                    })} flex items-center gap-2"
                                >
                                    <span>+</span>
                                    <span>New</span>
                                </button>
                            </div>
                        </div>
                        <div
                            class="grid grid-cols-[auto_1fr_auto_auto] divide-y divide-surface-border mt-2"
                        >
                            <div class="col-span-full grid grid-cols-subgrid items-center *:p-2">
                                <div>
                                    <svg width="8" height="8" viewBox="0 0 8 8" fill="none"
                                        ><path
                                            d="M1.5 4l2 2 3-3"
                                            stroke="var(--color-primary-fg)"
                                            stroke-width="1.5"
                                            stroke-linecap="round"
                                            stroke-linejoin="round"
                                        /></svg
                                    >
                                </div>
                                <span class="line-through text-fg-muted">Update homepage copy</span>
                                <div class="text-right">
                                    <span
                                        class="task-tag tag-blue bg-violet-50 dark:bg-base text-violet-500 p-1 rounded-md text-sm"
                                    >
                                        Website Redesign
                                    </span>
                                </div>
                                <div>
                                    <div
                                        class="p-1 text-sm rounded-full bg-green-500 text-green-50"
                                    >
                                        DD
                                    </div>
                                </div>
                            </div>
                            <div class="col-span-full grid grid-cols-subgrid items-center *:p-2">
                                <div>
                                    <svg width="8" height="8" viewBox="0 0 8 8" fill="none"
                                        ><path
                                            d="M1.5 4l2 2 3-3"
                                            stroke="var(--color-primary-fg)"
                                            stroke-width="1.5"
                                            stroke-linecap="round"
                                            stroke-linejoin="round"
                                        /></svg
                                    >
                                </div>
                                <span class="line-through text-fg-muted"
                                    >Adapt modern flat design</span
                                >
                                <div class="text-right">
                                    <span
                                        class="task-tag tag-blue bg-violet-50 dark:bg-base text-violet-500 p-1 rounded-md text-sm"
                                    >
                                        Website Redesign
                                    </span>
                                </div>
                                <div>
                                    <div
                                        class="p-1 text-sm rounded-full bg-amber-500 text-amber-50 text-center"
                                    >
                                        TP
                                    </div>
                                </div>
                            </div>
                            <div class="col-span-full grid grid-cols-subgrid items-center *:p-2">
                                <div></div>
                                <span>Re-imagine dashboard sidebar layout</span>
                                <div class="text-right">
                                    <span
                                        class="task-tag tag-blue bg-violet-50 dark:bg-base text-violet-500 p-1 rounded-md text-sm"
                                    >
                                        Website Redesign
                                    </span>
                                </div>
                                <div>
                                    <div
                                        class="p-1 text-sm rounded-full bg-green-500 text-green-50"
                                    >
                                        DD
                                    </div>
                                </div>
                            </div>
                            <div class="col-span-full grid grid-cols-subgrid items-center *:p-2">
                                <div></div>
                                <span>Performance audit — Lighthouse score</span>
                                <div class="text-right">
                                    <span
                                        class="task-tag tag-blue bg-violet-50 dark:bg-base text-violet-500 p-1 rounded-md text-sm"
                                    >
                                        Website Redesign
                                    </span>
                                </div>
                                <div>
                                    <div
                                        class="p-1 text-sm rounded-full bg-amber-500 text-amber-50 text-center"
                                    >
                                        TP
                                    </div>
                                </div>
                            </div>
                            <div class="col-span-full grid grid-cols-subgrid items-center *:p-2">
                                <div></div>
                                <span>Improve mobile responsiveness</span>
                                <div class="text-right">
                                    <span
                                        class="task-tag tag-blue bg-violet-50 dark:bg-base text-violet-500 p-1 rounded-md text-sm"
                                    >
                                        Website Redesign
                                    </span>
                                </div>
                                <div>
                                    <div
                                        class="p-1 text-sm rounded-full bg-green-500 text-green-50"
                                    >
                                        DD
                                    </div>
                                </div>
                            </div>
                            <div class="col-span-full grid grid-cols-subgrid items-center *:p-2">
                                <div></div>
                                <span>Content migration & Cleanup</span>
                                <div class="text-right">
                                    <span
                                        class="task-tag tag-blue bg-violet-50 dark:bg-base text-violet-500 p-1 rounded-md text-sm"
                                    >
                                        Website Redesign
                                    </span>
                                </div>
                                <div>
                                    <div
                                        class="p-1 text-sm rounded-full bg-green-500 text-green-50"
                                    >
                                        DD
                                    </div>
                                </div>
                            </div>
                            <div class="col-span-full grid grid-cols-subgrid items-center *:p-2">
                                <div></div>
                                <span>UX improvements for checkout flow</span>
                                <div class="text-right">
                                    <span
                                        class="task-tag tag-blue bg-violet-50 dark:bg-base text-violet-500 p-1 rounded-md text-sm"
                                    >
                                        Website Redesign
                                    </span>
                                </div>
                                <div>
                                    <div
                                        class="p-1 text-sm rounded-full bg-amber-500 text-amber-50 text-center"
                                    >
                                        TP
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <hr class="border-surface-border" />
    <div class="fade-up-slow" style="--_fade-up-delay: .5s;">
        <FeaturesSection />
    </div>
</main>
<Footer />

<style>
    @keyframes hero-visual-parallax {
        from {
            transform: translateY(12rem);
        }
        to {
            transform: translateY(0);
        }
    }

    .hero-visual {
        padding-bottom: 4rem;
        animation: hero-visual-parallax ease;
        animation-timeline: scroll();
        animation-range: normal 500px;
        will-change: transform;
    }

    @keyframes landing-fade-up {
        from {
            opacity: 0;
            transform: translateY(12px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    .fade-up {
        animation: landing-fade-up 0.8s var(--_fade-up-delay) ease both;
    }
    .fade-up-slow {
        animation: landing-fade-up 1.2s var(--_fade-up-delay) ease both;
    }
</style>
