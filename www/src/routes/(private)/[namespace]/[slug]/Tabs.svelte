<script lang="ts">
    import { afterNavigate, beforeNavigate } from '$app/navigation';
    import { page } from '$app/state';
    import { Tabs } from '~/lib/components/builders.svelte';
    import { Dashboard, DashboardOutline, TaskIcon, TaskIconOutline } from '~/lib/components/icons';
    import { Flip, gsap } from '~/lib/utils/gsap';
    import { C } from '~/lib/utils/styles';

    gsap.registerPlugin(Flip);

    const { tabs }: { tabs: Tabs } = $props();
    const id = $props.id();
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

    let flipState: Flip.FlipState | null = null;

    beforeNavigate(() => {
        flipState = Flip.getState(`#active-${id}`, { simple: true });
    });

    afterNavigate(() => {
        if (!flipState) {
            return;
        }

        Flip.from(flipState, {
            targets: `#active-${id}`,
            absolute: true,
            duration: 0.2,
            ease: 'circ.inOut',
            clearProps: 'transform',
            onComplete() {
                flipState = null;
            },
        });
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
                        id="active-{id}"
                        data-flip-id="active-{id}"
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
