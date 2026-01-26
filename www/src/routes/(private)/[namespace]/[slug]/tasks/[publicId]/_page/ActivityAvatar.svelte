<script lang="ts">
    import Avatar from '~/lib/components/Avatar.svelte';
    import type { UserPreset } from '~/lib/models/user';
    import { namespaceUrl } from '~/lib/utils/url';

    const { user }: { user: UserPreset['Avatar'] } = $props();
</script>

<a href={namespaceUrl(user.name)} class="flex gap-2 items-center c-link">
    <Avatar {user} class="size-avatar-xs shrink-0 rounded-full" />
    <span><strong>{user.displayName ?? user.name}</strong></span>
</a>
