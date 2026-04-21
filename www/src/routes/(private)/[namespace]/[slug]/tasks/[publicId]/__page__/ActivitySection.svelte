<script lang="ts">
    import { busy } from '@duydang2311/svutils';
    import { onDestroy } from 'svelte';
    import ActivityFeed from '~/lib/components/activity-feed/ActivityFeed.svelte';
    import { Loader } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import { guardNull } from '~/lib/utils/guard';
    import ActivityShell from './ActivityShell.svelte';
    import AddComment from './AddComment.svelte';
    import CommentedActivity from './CommentedActivity.svelte';
    import { useActivityLists, usePageContext, useTask, validators } from './utils.svelte';
    import { boolAttr } from 'runed';

    const ctx = usePageContext();
    const task = $derived(useTask());
    const listBusy = busy();
    const stuck = $derived(listBusy.for(1000));
    const activityLists = $derived(useActivityLists(ctx.activityListParams));
    const activities = $derived(activityLists.flatMap((a) => a.query.current?.items ?? []));

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
    } as const;
</script>

<section class="mt-10 max-w-container-lg mx-auto">
    <div data-stuck={boolAttr(stuck)} class="data-stuck:animate-pulse relative">
        {#if activities.length > 0}
            <ActivityFeed
                {activities}
                {renderers}
                shell={ActivityShell}
                class="relative flex flex-col gap-6 duration-300 animate-fade-in"
            >
                <div
                    class="absolute h-full w-px bg-surface-border left-[calc(var(--spacing-avatar-xs)/2+var(--spacing)*3)] top-0"
                ></div>
            </ActivityFeed>
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

                const lastList = activityLists.at(-1);
                if (!lastList || lastList.query.loading || !lastList.query.current?.hasNext) {
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
