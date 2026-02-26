<script lang="ts">
    import { browser } from '$app/environment';
    import { busy } from '@duydang2311/svutils';
    import { circInOut } from 'svelte/easing';
    import { fly } from 'svelte/transition';
    import invariant from 'tiny-invariant';
    import { usePageData } from '~/lib/utils/kit';
    import { useRuntime } from '~/lib/utils/runtime';
    import type { PageData } from './$types';
    import RenderActivity from './_page/RenderActivity.svelte';
    import { getActivityList, usePageContext } from './_page/utils.svelte';

    const { http } = useRuntime();
    const data = usePageData<PageData>();
    const ctx = usePageContext();
    const activityListBusy = busy();

    if (!ctx.activityList) {
        activityListBusy.set();
    }

    if (browser) {
        (async () => {
            if (!ctx.activityList) {
                invariant(data.session, 'data.session must not be null');
                ctx.activityList = await getActivityList(http)(data.session.user.id).finally(() => {
                    activityListBusy.unset();
                });
            } else {
                activityListBusy.unset();
            }
        })();
    }
</script>

<div>
    <h2 class="text-body-sm text-fg-emph">Feed</h2>
    {#if ctx.activityList && ctx.activityList.items.length}
        <ol
            class="flex flex-col gap-4 mt-2"
            transition:fly={{ y: -2, duration: 200, easing: circInOut }}
        >
            {#each ctx.activityList.items as activity (activity.id)}
                <RenderActivity {activity} />
            {/each}
        </ol>
    {:else if activityListBusy.now}
        <p class="text-fg-muted text-sm mt-2">Loading activities...</p>
    {/if}
</div>
