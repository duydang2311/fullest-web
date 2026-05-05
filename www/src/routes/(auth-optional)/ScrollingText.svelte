<script lang="ts">
    import { untrack } from 'svelte';
    import { gsap } from '~/lib/utils/gsap';

    const texts = ['devs', 'creators', 'builders', 'everyone'];
    let index = 0;
</script>

<div class="leading-none">
    <span>Task management for</span>
    <span
        class="inline-block text-primary-fg align-top"
        {@attach (node) => {
            return untrack(() => {
                const interval = setInterval(() => {
                    index = (index + 1) % texts.length;
                    const oldWidth = node.clientWidth;
                    const oldContent = node.textContent;
                    node.textContent = texts[index];
                    const tl = gsap.timeline();
                    const newWidth = node.clientWidth;
                    node.textContent = oldContent;
                    tl.set(node, { width: oldWidth })
                        .to(node, {
                            opacity: 0,
                            yPercent: -10,
                            scaleY: 1.1,
                            filter: 'blur(1px)',
                            duration: 0.2,
                            ease: 'circ.out',
                            onComplete() {
                                node.textContent = texts[index];
                            },
                        })
                        .set(node, {
                            yPercent: 30,
                            scaleY: 1.4,
                            filter: 'blur(1px)',
                        })
                        .to(
                            node,
                            {
                                width: newWidth,
                                duration: 0.2,
                                ease: 'sine.inOut',
                            },
                            'animateWidth'
                        )
                        .to(
                            node,
                            {
                                yPercent: 0,
                                scaleY: 1,
                                filter: 'none',
                                duration: 0.2,
                                ease: 'circ.out',
                            },
                            'animateWidth+=0.15'
                        )
                        .to(
                            node,
                            {
                                opacity: 1,
                                duration: 0.2,
                                ease: 'circ.out',
                            },
                            'animateWidth+=0.15'
                        )
                        .set(node, {
                            clearProps: 'all',
                        });
                }, 3000);

                return () => clearInterval(interval);
            });
        }}
    >
        {texts[index]}
    </span>
    <!-- <span class="-ml-2">.</span> -->
</div>
