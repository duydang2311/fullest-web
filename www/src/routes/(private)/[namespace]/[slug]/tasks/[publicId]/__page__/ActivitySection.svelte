<script lang="ts">
    import { busy } from '@duydang2311/svutils';
    import { boolAttr } from 'runed';
    import { onDestroy } from 'svelte';
    import ActivityFeed from '~/lib/components/activity-feed/ActivityFeed.svelte';
    import { activityValidators } from '~/lib/components/activity-feed/utils.svelte';
    import { Loader } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import { guardNull } from '~/lib/utils/guard';
    import ActivityCreated from './ActivityCreated.svelte';
    import ActivityShell from './ActivityShell.svelte';
    import AddComment from './AddComment.svelte';
    import CommentedActivity from './CommentedActivity.svelte';
    import { useActivityLists, usePageContext, useTask, validators } from './utils.svelte';

    const ctx = usePageContext();
    const task = $derived(useTask());
    const listBusy = busy();
    const stuck = $derived(listBusy.for(1000));
    const activityQueries = $derived(useActivityLists(ctx.activityListParams));
    const activityLists = $derived(
        await Promise.all(
            activityQueries.map(async (a) => ({
                param: a.param,
                list: await a.query,
            }))
        )
    );
    const activities = $derived.by(() => {
        const seen = new Set();
        const result = [];
        for (const a of activityLists) {
            for (const item of a.list.items) {
                if (!seen.has(item.id)) {
                    seen.add(item.id);
                    result.push(item);
                }
            }
        }
        return result;
    });

    async function loadMore() {
        const currentTask = task.current;
        guardNull(currentTask);
        const lastItem = activities.at(-1);
        guardNull(lastItem);
        ctx.activityListParams.push({
            taskId: currentTask.id,
            size: 20,
            afterId: lastItem.id,
        });
    }

    const renderers = {
        [ActivityKind.Commented]: {
            validator: validators.commented,
            component: CommentedActivity,
        },
        [ActivityKind.Created]: {
            validator: activityValidators.created,
            component: ActivityCreated,
        },
    } as const;
</script>

<section class="max-w-container-lg mx-auto">
    <h2 class="text-fg-emph text-body-sm">Activity</h2>
    <div data-stuck={boolAttr(stuck)} class="data-stuck:animate-pulse relative mt-4">
        {#if activities.length > 0}
            <ActivityFeed
                {activities}
                {renderers}
                shell={ActivityShell}
                class="flex flex-col gap-6 duration-300 animate-fade-in text-fg-dim"
            />
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
                if (!entries[0]?.isIntersecting) {
                    return;
                }

                const lastQuery = activityQueries.at(-1);
                if (!lastQuery || lastQuery.query.loading || !lastQuery.query.current?.hasNext) {
                    return;
                }

                loadMore();
            });
            onDestroy(() => {
                observer.disconnect();
            });
            return (node) => {
                activities;
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
