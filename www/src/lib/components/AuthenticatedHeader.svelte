<script lang="ts">
    import type { UserPreset } from '../models/user';
    import { button } from '../utils/styles';
    import CreateActions from './AuthenticatedHeaderCreateActions.svelte';
    import AuthenticatedHeaderPfp from './AuthenticatedHeaderPfp.svelte';
    import LogoType from './LogoType.svelte';

    interface Props {
        user: UserPreset['Avatar'];
    }

    const { user }: Props = $props();
</script>

<header class="px-2 lg:px-4 h-12 content-center">
    <nav class="flex items-center justify-between gap-8">
        <div class="flex items-center gap-2">
            <LogoType />
            <a href="/" class={button({ variant: 'base', ghost: true })}>Dashboard</a>
        </div>
        <div class="flex items-center gap-2">
            <CreateActions />
            <AuthenticatedHeaderPfp {user} />
        </div>
    </nav>
</header>
