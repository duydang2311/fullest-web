<script lang="ts">
    import { goto } from '$app/navigation';
    import { portal } from '@zag-js/svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import {
        ChevronDownOutline,
        PlusOutline,
        CubeOutline,
        CircleDotOutline,
    } from '~/lib/components/icons';
    import { button } from '~/lib/utils/styles';

    const id = $props.id();
    const menu = createMenu({
        id,
        onSelect: async (details) => {
            await goto(details.value);
        },
    });
</script>

<button
    {...menu.api.getTriggerProps()}
    class="{button({
        variant: 'base',
        ghost: true,
        size: 'sm',
    })} flex items-center gap-1"
>
    <PlusOutline />
    <ChevronDownOutline />
</button>
<div use:portal {...menu.api.getPositionerProps()}>
    <ul {...menu.api.getContentProps()} class="c-menu-content flex flex-col gap-1 min-w-40">
        {#each [{ icon: CubeOutline, href: '/new', label: 'Create project' }] as item (item.href)}
            <li>
                <a
                    href={item.href}
                    {...menu.api.getItemProps({ value: item.href })}
                    class="c-menu-item flex items-center gap-2"
                >
                    <item.icon />
                    <span>
                        {item.label}
                    </span>
                </a>
            </li>
        {/each}
    </ul>
</div>
