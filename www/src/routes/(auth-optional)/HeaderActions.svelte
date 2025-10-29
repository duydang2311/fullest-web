<script lang="ts">
    import { goto } from '$app/navigation';
    import { portal } from '@zag-js/svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { MenuOutline } from '~/lib/components/icons';
    import { button } from '~/lib/utils/styles';

    const id = $props.id();
    const menu = createMenu({
        id,
        onSelect: async (details) => {
            await goto(details.value);
        },
    });
</script>

<div class="hidden items-center gap-2 sm:flex">
    <a href="/sign-in" class={button({ variant: 'base', ghost: true })}> Sign in </a>
    <a href="/sign-up" class={button({ variant: 'primary', filled: true })}> Sign up </a>
</div>
<div class="sm:hidden">
    <button
        {...menu.api.getTriggerProps()}
        class={button({ variant: 'base', icon: true, ghost: true })}
    >
        <MenuOutline />
    </button>
    <div use:portal {...menu.api.getPositionerProps()}>
        <ul {...menu.api.getContentProps()} class="c-menu--content flex flex-col gap-1">
            {#each [{ href: '/sign-in', label: 'Sign in' }, { href: '/sign-up', label: 'Sign up' }] as item (item.href)}
                <li>
                    <a
                        {...menu.api.getItemProps({ value: item.href })}
                        href={item.href}
                        class="c-menu--item"
                    >
                        {item.label}
                    </a>
                </li>
            {/each}
        </ul>
    </div>
</div>
