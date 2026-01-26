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

<header class="border-b-base-border bg-base-light dark:bg-base-dark border-b px-8 py-2">
    <nav class="max-w-container-xl mx-auto flex w-full items-center justify-between gap-8">
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
