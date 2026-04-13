<script lang="ts">
    import { onNavigate } from '$app/navigation';
    import { page } from '$app/state';
    import { animate, type AnimationPlaybackControls } from 'motion';
    import { Tabs } from '~/lib/components/builders.svelte';
    import { Dashboard, DashboardOutline, TaskIcon, TaskIconOutline } from '~/lib/components/icons';
    import { C } from '~/lib/utils/styles';

    let activeEl = $state.raw<HTMLElement>();
    const { tabs }: { tabs: Tabs } = $props();
    const tabItems = [
        {
            icon: DashboardOutline,
            activeIcon: Dashboard,
            label: 'Overview',
            href: `/${page.params.namespace}/${page.params.slug}`,
        },
        {
            icon: TaskIconOutline,
            activeIcon: TaskIcon,
            label: 'Tasks',
            href: `/${page.params.namespace}/${page.params.slug}/tasks`,
        },
    ];

    let anim: AnimationPlaybackControls | null = null;
    onNavigate(() => {
        if (!activeEl) {
            return;
        }
        const lastEl = activeEl;
        const first = activeEl.getBoundingClientRect();
        return () => {
            if (!activeEl || lastEl === activeEl) {
                return;
            }
            const last = activeEl.getBoundingClientRect();
            const dx = first.left - last.left;
            const dy = first.top - last.top;
            const dw = last.width ? first.width / last.width : 1;
            const dh = last.height ? first.height / last.height : 1;
            if (anim) {
                anim.stop();
            }
            anim = animate(
                activeEl,
                {
                    x: [dx, 0],
                    y: [dy, 0],
                    scaleX: [dw, 0.98, 1],
                    scaleY: [dh, 1.1, 1],
                },
                {
                    duration: 0.2,
                    ease: 'circInOut',
                    onComplete() {
                        anim = null;
                    },
                }
            );
        };
    });
</script>

<div {...tabs.getRootProps()}>
    <div {...tabs.getListProps()} class="flex flex-col items-center">
        {#each tabItems as item (item.href)}
            {@const active = tabs.value === item.href}
            <button
                {...tabs.getTriggerProps({ value: item.href })}
                class="group relative w-full text-left rounded-md focus-visible:outline-none focus-visible:ring focus-visible:ring-base-border"
            >
                {#if active}
                    <div
                        bind:this={activeEl}
                        class="bg-base-emph rounded-md absolute top-0 left-0 size-full"
                    ></div>
                {/if}
                <a
                    href={item.href}
                    class="{C.button({
                        variant: 'base',
                        ghost: true,
                    })} z-1 relative flex items-center gap-4 font-normal group-data-selected:font-medium bg-transparent"
                    data-sveltekit-keepfocus
                    data-sveltekit-replacestate
                >
                    {#if active}
                        <item.activeIcon />
                    {:else}
                        <item.icon />
                    {/if}
                    <span>
                        {item.label}
                    </span>
                </a>
            </button>
        {/each}
    </div>
</div>
