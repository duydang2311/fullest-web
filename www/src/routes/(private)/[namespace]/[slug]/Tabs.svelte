<script lang="ts">
    import { page } from '$app/state';
    import { Tabs } from '~/lib/components/builders.svelte';
    import { button } from '~/lib/utils/styles';

    const { tabs }: { tabs: Tabs } = $props();
    const tabItems = [
        { label: 'Overview', href: `/${page.params.namespace}/${page.params.slug}` },
        { label: 'Tasks', href: `/${page.params.namespace}/${page.params.slug}/tasks` },
    ];
</script>

<div {...tabs.getRootProps()} class="h-full">
    <div {...tabs.getListProps()} class="flex items-center h-full">
        {#each tabItems as item (item.href)}
            <button
                {...tabs.getTriggerProps({ value: item.href })}
                class="focus-visible:ring-focus-base-fg group relative focus-visible:outline-none min-w-max px-0 h-full overflow-hidden"
            >
                {#if tabs.value === item.href}
                    <div
                        class="bg-base-active absolute size-full animate-slide-in-from-b duration-300"
                    ></div>
                {/if}
                <a
                    href={item.href}
                    class="{button({
                        variant: 'base',
                    })} content-center font-normal px-4 py-2 h-full tracking-tight relative text-base-fg-muted hover:text-base-fg rounded-b-none group-data-selected:bg-transparent group-data-selected:text-base-fg-strong block group-data-selected:font-bold"
                >
                    {item.label}
                </a>
            </button>
        {/each}
    </div>
</div>
