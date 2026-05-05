<script lang="ts">
    import { browser } from '$app/environment';
    import { goto } from '$app/navigation';
    import { page } from '$app/state';
    import { busy } from '@duydang2311/svutils';
    import { boolAttr } from 'runed';
    import { circInOut } from 'svelte/easing';
    import { fly } from 'svelte/transition';
    import invariant from 'tiny-invariant';
    import ActivityFeed from '~/lib/components/activity-feed/ActivityFeed.svelte';
    import { reverseKeysetList } from '~/lib/models/paginated';
    import { usePageData } from '~/lib/utils/kit';
    import { useRuntime } from '~/lib/utils/runtime';
    import { C } from '~/lib/utils/styles';
    import { fluentSearchParams } from '~/lib/utils/url';
    import type { PageData } from './$types';
    import { makeFetchActivityList, usePageContext } from './__page__/utils.svelte';

    const { http } = useRuntime();
    const data = usePageData<PageData>();
    const ctx = usePageContext();
    const activityListBusy = busy();
    const fetchActivityList = makeFetchActivityList(http);

    async function loadMore(direction: 'forward' | 'backward') {
        invariant(data.session, 'data.session must not be null');
        invariant(ctx.activityList, 'ctx.activityList must not be null');
        activityListBusy.set();
        try {
            if (direction === 'forward') {
                const lastItem = ctx.activityList.items.at(-1);
                invariant(lastItem, 'lastItem must not be null');
                const list = await fetchActivityList(
                    data.session.user.id,
                    lastItem.id,
                    null,
                    'desc',
                    20
                );
                ctx.activityList = {
                    items: [...ctx.activityList.items, ...list.items],
                    hasPrevious: ctx.activityList.hasPrevious,
                    hasNext: list.hasNext,
                    totalCount: 0,
                };
                await goto(
                    fluentSearchParams(page.url.searchParams)
                        .set('a', list.items.at(-1)?.id)
                        .toSearch(),
                    { replaceState: true }
                );
            } else {
                const firstItem = ctx.activityList.items[0];
                invariant(firstItem, 'firstItem must not be null');
                const list = reverseKeysetList(
                    await fetchActivityList(data.session.user.id, firstItem.id, null, 'asc', 20)
                );
                ctx.activityList = {
                    items: [...list.items, ...ctx.activityList.items],
                    hasPrevious: list.hasPrevious,
                    hasNext: ctx.activityList.hasNext,
                    totalCount: 0,
                };
            }
        } finally {
            activityListBusy.unset();
        }
    }

    if (!ctx.activityList) {
        activityListBusy.set();
    }

    if (browser) {
        (async () => {
            if (!ctx.activityList) {
                const lastId = page.url.searchParams.get('a');
                invariant(data.session, 'data.session must not be null');
                const list = await fetchActivityList(
                    data.session.user.id,
                    null,
                    lastId,
                    'desc',
                    lastId ? 60 : 20
                ).finally(() => {
                    activityListBusy.unset();
                });
                ctx.activityList = lastId ? reverseKeysetList(list) : list;
            } else {
                activityListBusy.unset();
            }
        })();
    }
</script>

<div>
    <h2 class="text-body-sm text-fg-emph">Feed</h2>
    {#if ctx.activityList && ctx.activityList.items.length}
        {#if ctx.activityList.hasPrevious}
            <button
                type="button"
                class="{C.button({ filled: true, size: 'sm' })} my-2"
                disabled={activityListBusy.now}
                onclick={() => {
                    loadMore('backward');
                }}
            >
                Load more
            </button>
        {/if}
        <div transition:fly={{ y: -2, duration: 200, easing: circInOut }}>
            <ActivityFeed
                data-busy={boolAttr(activityListBusy.for(1000))}
                activities={ctx.activityList.items}
                class="flex flex-col gap-4 mt-2 data-busy:animate-pulse"
            />
        </div>
        {#if ctx.activityList.hasNext}
            <button
                type="button"
                class="{C.button({ filled: true, size: 'sm' })} mt-4"
                disabled={activityListBusy.now}
                onclick={() => {
                    loadMore('forward');
                }}
            >
                Load more
            </button>
        {/if}
    {:else if activityListBusy.now}
        <p class="text-fg-muted text-sm mt-2">Loading activities...</p>
    {/if}
</div>
