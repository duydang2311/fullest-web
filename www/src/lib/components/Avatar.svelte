<script lang="ts">
    import { pixelArt } from '@dicebear/collection';
    import { createAvatar } from '@dicebear/core';
    import type { HTMLAttributes } from 'svelte/elements';
    import type { User } from '../models/user';
    import { buildImageUrl } from '../utils/asset';

    interface Props extends HTMLAttributes<HTMLElement> {
        user?: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
    }

    const { user, class: cls, ...props }: Props = $props();
</script>

{#if user}
    <img
        src={user.imageKey && user.imageVersion
            ? buildImageUrl(user.imageKey, user.imageVersion)
            : createAvatar(pixelArt, { seed: user.name, scale: 200 }).toDataUri()}
        alt={user.displayName ?? user.name}
        class="c-avatar {cls}"
        {...props}
    />
{:else}
    <div class="c-avatar--fallback {cls}"></div>
{/if}
