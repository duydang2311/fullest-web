<script lang="ts">
    import { page } from '$app/state';
    import { sineInOut } from 'svelte/easing';
    import { fly } from 'svelte/transition';
    import { Tabs } from '~/lib/components/builders.svelte';
    import { button } from '~/lib/utils/styles';

    const { tabs }: { tabs: Tabs } = $props();
    const tabItems = [
        { label: 'Overview', href: `/${page.params.namespace}/${page.params.slug}` },
        { label: 'Tasks', href: `/${page.params.namespace}/${page.params.slug}/tasks` },
        { label: 'Activity', href: `/${page.params.namespace}/${page.params.slug}/activity` },
        {
            label: 'Contributors',
            href: `/${page.params.namespace}/${page.params.slug}/contributors`,
        },
    ];
</script>

<div {...tabs.getRootProps()}>
    <div
        {...tabs.getListProps()}
        class="border-b-base-border flex items-center border-b px-8 *:basis-28"
    >
        {#each tabItems as item}
            <button
                {...tabs.getTriggerProps({ value: item.href })}
                class="focus-visible:ring-focus-base-fg rounded-t-lg group relative focus-visible:outline-none overflow-hidden min-w-max"
            >
                {#if tabs.value === item.href}
                    <div
                        class="bg-secondary absolute inset-0 rounded-t-lg animate-slide-in-from-b duration-300"
                        out:fly={{ y: '100%', duration: 300, easing: sineInOut }}
                    ></div>
                {/if}
                <a
                    href={item.href}
                    class="{button({
                        variant: 'base',
                        ghost: true,
                    })} relative active:translate-y-0.5 rounded-b-none group-data-selected:bg-transparent group-data-selected:text-base-fg-strong block group-data-selected:font-bold"
                >
                    {item.label}
                </a>
            </button>
        {/each}
    </div>
</div>
