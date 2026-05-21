<script lang="ts">
    import type { UserPreset } from '../models/user';
    import AuthenticatedHeaderNav from './AuthenticatedHeaderNav.svelte';

    interface Props {
        user: UserPreset['Avatar'];
    }

    const { user }: Props = $props();
</script>

<header class="px-2 lg:px-4 h-16 content-center">
    <AuthenticatedHeaderNav {user} />
</header>
