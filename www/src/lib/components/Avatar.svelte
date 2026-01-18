<script lang="ts">
    import type { HTMLAttributes } from 'svelte/elements';
    import type { User } from '../models/user';
    import { buildImageUrl } from '../utils/asset';

    interface Props extends HTMLAttributes<HTMLElement> {
        user?: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
    }

    const { user, class: cls, ...props }: Props = $props();
</script>

{#if user && user.imageKey && user.imageVersion}
    <img
        src={buildImageUrl(user.imageKey, user.imageVersion)}
        alt={user.displayName ?? user.name}
        class={cls}
        {...props}
    />
{:else}
    <div class="c-avatar--fallback {cls}"></div>
{/if}
