<script lang="ts">
    import { portal } from '@zag-js/svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { LogOutOutline } from '~/lib/components/icons';
    import { button } from '~/lib/utils/styles';
    import type { UserPreset } from '../models/user';
    import Avatar from './Avatar.svelte';

    interface Props {
        user: UserPreset['Avatar'];
    }

    const { user }: Props = $props();

    const id = $props.id();
    const menu = createMenu({ id });
</script>

<div class="shrink-0 flex items-center py-0.5">
    <button
        {...menu.api.getTriggerProps()}
        class="{button({
            variant: 'base',
            icon: true,
            outlined: true,
            ghost: true,
        })} rounded-full p-0"
    >
        <Avatar {user} class="size-avatar-sm rounded-full" />
    </button>
    <div use:portal {...menu.api.getPositionerProps()}>
        <ul {...menu.api.getContentProps()} class="c-menu-content flex flex-col gap-1">
            {#each [{ icon: LogOutOutline, href: '/sign-out', label: 'Log out' }] as item (item.href)}
                <li>
                    <a
                        {...menu.api.getItemProps({ value: item.href })}
                        href={item.href}
                        class="c-menu-item flex items-center gap-4"
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
</div>
