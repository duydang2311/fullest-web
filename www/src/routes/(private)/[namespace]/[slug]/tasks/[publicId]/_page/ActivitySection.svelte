<script lang="ts">
    import { busy } from '@duydang2311/svutils';
    import { onDestroy } from 'svelte';
    import invariant from 'tiny-invariant';
    import { Loader } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import { usePageData } from '~/lib/utils/kit';
    import { useRuntime } from '~/lib/utils/runtime';
    import type { PageData } from '../$types';
    import ActivityItem from './ActivityItem.svelte';
    import AddComment from './AddComment.svelte';
    import { fetchActivityList, usePageContext } from './utils.svelte';

    const data = usePageData<PageData>();
    const { http } = useRuntime();
    const ctx = usePageContext();
    const listBusy = busy();
    const activities = $derived(
        ctx.activityList?.items.filter((a) => {
            if (a.kind !== ActivityKind.Commented) {
                return true;
            }
            invariant(
                typeof a.metadata === 'object' &&
                    a.metadata &&
                    'comment' in a.metadata &&
                    a.metadata.comment &&
                    typeof a.metadata.comment === 'object' &&
                    a.metadata.comment &&
                    'id' in a.metadata.comment &&
                    typeof a.metadata.comment.id === 'string',
                'activity.metadata.comment.id must be string'
            );
            return a.metadata.comment.id !== data.task.initialCommentId;
        }) ?? []
    );
    const stuck = $derived(listBusy.for(1000));

    async function loadMore() {
        listBusy.set();
        const list = await fetchActivityList(http)(data.task.id, ctx.activityList?.cursor).finally(
            () => listBusy.unset()
        );
        ctx.activityList = {
            ...list,
            items: [...(ctx.activityList?.items ?? []), ...list.items],
        };
        // const last = ctx.activityList.items.at(-1);
        // if (last) {
        //     const url = new URL(page.url);
        //     url.searchParams.set('cursor', last.id);
        //     replaceState(url, page.state);
        // }
    }
</script>

<section class="mt-8 max-w-container-lg mx-auto">
    <div data-stuck={stuck ? '' : undefined} class="data-stuck:animate-pulse relative">
        {#if activities.length > 0}
            <ol class="mt-4 flex flex-col gap-6 animate-slide-in-from-b duration-300 slide-[1rem]">
                {#each activities as activity (activity.id)}
                    <ActivityItem {activity} />
                {/each}
            </ol>
        {/if}
        {#if stuck}
            <div class="absolute left-1/2 -translate-x-1/2 bottom-0">
                <Loader class="animate-spin" />
            </div>
        {/if}
    </div>
    <div
        {@attach (() => {
            const observer = new IntersectionObserver((entries) => {
                if (!entries[0]?.isIntersecting || !ctx.activityList?.hasMore || listBusy.now) {
                    return;
                }

                loadMore();
            });
            onDestroy(() => {
                observer.disconnect();
            });
            return (node) => {
                ctx.activityList;
                observer.observe(node);
                return () => {
                    observer.unobserve(node);
                };
            };
        })()}
    ></div>
    <div class="mt-6">
        <AddComment />
    </div>
</section>
