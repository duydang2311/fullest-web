<script lang="ts">
    import { afterNavigate, beforeNavigate } from '$app/navigation';
    import type { Snippet } from 'svelte';
    import { gsap } from '~/lib/utils/gsap';

    const { top, left, main }: { top: Snippet; left: Snippet; main: Snippet } = $props();
    let navigateIndicatorEl = $state.raw<HTMLDivElement>();
    let finishTl: gsap.core.Timeline | null = null;

    beforeNavigate((nav) => {
        if (!navigateIndicatorEl || nav.type !== 'link' || !nav.to) {
            return;
        }

        const el = navigateIndicatorEl;
        gsap.killTweensOf(el);
        if (finishTl) {
            finishTl.kill();
            finishTl = null;
        }
        gsap.set(el, {
            scaleX: 0,
            scaleY: 1,
        });

        function step() {
            const current = gsap.getProperty(el, 'scaleX') as number;
            if (current >= 0.7) return;
            const remaining = 0.7 - current;
            const delta = remaining * (0.2 + Math.random() * 0.3);
            const next = current + delta;
            gsap.to(el, {
                scaleX: next,
                duration: 0.3 + Math.random() * 0.7,
                ease: 'power1.out',
                onComplete: step,
            });
        }

        step();
    });

    afterNavigate((nav) => {
        if (!navigateIndicatorEl || !nav.from || nav.type !== 'link') {
            return;
        }

        const el = navigateIndicatorEl;
        gsap.killTweensOf(el);
        finishTl = gsap
            .timeline()
            .to(el, {
                scaleX: 1,
                duration: 0.3,
                ease: 'sine',
            })
            .to(el, {
                scaleY: 0,
                duration: 0.4,
                ease: 'sine',
            })
            .add(() => {
                gsap.set(el, {
                    scaleX: 0,
                    scaleY: 1,
                    clearProps: 'transform',
                });
            });
    });
</script>

<div class="bg-surface-subtle dark:bg-surface-1 flex flex-col h-screen overflow-hidden">
    {@render top()}
    <div class="flex flex-1 overflow-hidden">
        <div class="min-w-72">
            {@render left()}
        </div>
        <div class="relative flex-1 flex flex-col overflow-hidden rounded-tl-2xl">
            <div
                class="flex-1 bg-surface border-t border-l border-surface-border rounded-tl-2xl flex flex-col overflow-auto"
            >
                {@render main()}
            </div>
            <div
                bind:this={navigateIndicatorEl}
                class="absolute w-full scale-x-0 h-0.5 top-0 bg-fg-muted origin-top-left"
            ></div>
        </div>
    </div>
</div>
