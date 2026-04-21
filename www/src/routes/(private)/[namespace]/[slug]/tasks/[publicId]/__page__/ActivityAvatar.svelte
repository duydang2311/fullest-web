<script lang="ts">
    import Avatar from '~/lib/components/Avatar.svelte';
    import type { UserPreset } from '~/lib/models/user';
    import { namespaceUrl } from '~/lib/utils/url';

    const { user }: { user: UserPreset['Avatar'] } = $props();
</script>

<a href={namespaceUrl(user.name)} class="shrink-0">
    <Avatar {user} class="size-avatar-xs rounded-full" />
</a>
