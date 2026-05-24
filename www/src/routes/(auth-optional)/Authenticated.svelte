<script lang="ts">
    import { DateTime } from 'luxon';
    import invariant from 'tiny-invariant';
    import AuthenticatedHeaderPfp from '~/lib/components/AuthenticatedHeaderPfp.svelte';
    import Card from '~/lib/components/Card.svelte';
    import Footer from '~/lib/components/Footer.svelte';
    import { Logo, PlusOutline } from '~/lib/components/icons';
    import { formatDate } from '~/lib/utils/date';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from './$types';
    import RecentActivities from './RecentActivities.svelte';
    import YourProjects from './YourProjects.svelte';

    const data = usePageData<PageData>();
    invariant(data.session, 'data.session must not be null');
    const user = $derived(data.session.user);
</script>

<header class="p-4 border-b border-surface-border flex gap-8 items-center justify-between text-sm">
    <nav class="flex items-center gap-2">
        <a
            href="/"
            aria-label="Home"
            class={C.button({ variant: 'base', ghost: true, size: 'sm' })}
        >
            <Logo />
        </a>
        <a href="/" class="{C.button({ ghost: true, size: 'sm' })} not-hover:text-fg-dim">Home</a>
    </nav>
    <AuthenticatedHeaderPfp {user} />
</header>
<main class="p-4">
    <div class="max-w-container-lg mx-auto">
        <h1 class="text-fg-emph text-title-sm">Good morning, {user.displayName ?? user.name} ☀️</h1>
        <p class="text-fg-dim text-sm">
            {formatDate(DateTime.now(), DateTime.DATE_MED_WITH_WEEKDAY)}
        </p>
        <div class="flex flex-col lg:flex-row gap-4 lg:items-start mt-8">
            <Card class="lg:flex-2">
                {#snippet header()}
                    <h2>Feed</h2>
                {/snippet}
                {#snippet body()}
                    <RecentActivities />
                {/snippet}
            </Card>
            <Card class="lg:flex-1">
                {#snippet header()}
                    <div class="flex items-center gap-4 justify-between">
                        <h2>Projects</h2>
                        <a
                            href="/new"
                            class="{C.button({
                                size: 'sm'
                            })} flex items-center gap-2 not-hover:text-fg-muted"
                        >
                            <PlusOutline />
                            <span>New</span>
                        </a>
                    </div>
                {/snippet}
                {#snippet body()}
                    <YourProjects />
                {/snippet}
            </Card>
        </div>
    </div>
</main>
<Footer />
