<script lang="ts">
    import { goto } from '$app/navigation';
    import { page } from '$app/state';
    import { createTabs } from '~/lib/components/builders.svelte';
    import { Dashboard, DashboardOutline, TaskIcon, TaskIconOutline } from '~/lib/components/icons';
    import { indexOf } from '~/lib/utils/string';
    import { C } from '~/lib/utils/styles';

    const TAB_ITEMS = [
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
        async onValueChange(details) {
            console.log('called goto');
            await goto(details.value, { keepFocus: true, replaceState: true, noScroll: true });
        },
        async navigate(details) {
            // note: keep this even though it's empty to prevent default tab trigger behaviour
        },
    });
    // console.log(Object.keys(tabs.getTriggerProps({value: '1'})))
</script>

<div {...tabs.getRootProps()}>
    <ul {...tabs.getListProps()} class="flex">
        {#each TAB_ITEMS as item (item.href)}
            {@const active = tabs.value === item.href}
            <li>
                <a
                    {...tabs.getTriggerProps({ value: item.href })}
                    onclick={(e) => {
                        e.preventDefault();
                        tabs.setValue(item.href);
                    }}
                    href={item.href}
                    class="{C.button({
                        variant: 'base',
                        ghost: true,
                    })} px-4 py-3 text-fg-muted relative flex items-center gap-2 font-normal data-selected:font-medium data-selected:text-fg-emph bg-transparent"
                    data-sveltekit-replacestate
                    data-sveltekit-noscroll
                >
                    {#if active}
                        <div
                            class="active-indicator bg-fg-emph h-[2px] absolute -bottom-0.5 left-0 w-full"
                        ></div>
                    {/if}
                    {#if active}
                        <item.activeIcon />
                    {:else}
                        <item.icon />
                    {/if}
                    <span>
                        {item.label}
                    </span>
                </a>
            </li>
        {/each}
    </ul>
</div>

<style>
    .active-indicator {
        transition:
            opacity 300ms ease,
            transform 300ms ease;
        transform-origin: center center;
        transform: none;
        opacity: unset;
        @starting-style {
            opacity: 0;
            transform: scaleX(0.8);
        }
    }
</style>
