<script lang="ts">
    import { goto } from '$app/navigation';
    import { portal } from '@zag-js/svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import {
        ChevronDownOutline,
        PlusOutline,
        ProjectOutline,
        TaskOutline,
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
        icon: true,
        outlined: true,
        filled: true,
    })} flex items-center gap-1"
>
    <PlusOutline />
    <ChevronDownOutline />
</button>
<div use:portal {...menu.api.getPositionerProps()}>
    <ul {...menu.api.getContentProps()} class="c-menu--content flex flex-col gap-1">
        {#each [{ icon: ProjectOutline, href: '/new', label: 'New project' }, { icon: TaskOutline, href: '/new-task', label: 'New task' }] as item (item.href)}
            <li>
                <a
                    href={item.href}
                    {...menu.api.getItemProps({ value: item.href })}
                    class="c-menu--item flex items-center gap-4"
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
