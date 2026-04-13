<script lang="ts">
    import { page } from '$app/state';
    import { Tabs } from '~/lib/components/builders.svelte';
    import { C } from '~/lib/utils/styles';

    const { tabs }: { tabs: Tabs } = $props();
    const tabItems = [
        { label: 'Overview', href: `/${page.params.namespace}/${page.params.slug}` },
        { label: 'Tasks', href: `/${page.params.namespace}/${page.params.slug}/tasks` },
    ];
</script>

<div {...tabs.getRootProps()}>
    <div {...tabs.getListProps()} class="flex flex-col items-center">
        {#each tabItems as item (item.href)}
            <button
                {...tabs.getTriggerProps({ value: item.href })}
                class="group relative w-full text-left rounded-md focus-visible:outline-none focus-visible:ring focus-visible:ring-base-border"
            >
                {#if tabs.value === item.href}
                    <div class="bg-base-emph rounded-md absolute top-0 left-0 size-full"></div>
                {/if}
                <a
                    href={item.href}
                    class="{C.button({
                        variant: 'base',
                        ghost: true,
                    })} relative block font-normal group-data-selected:font-medium"
                >
                    {item.label}
                </a>
            </button>
        {/each}
    </div>
</div>
