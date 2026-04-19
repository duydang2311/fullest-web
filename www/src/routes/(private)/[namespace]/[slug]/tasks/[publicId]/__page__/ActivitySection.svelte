<script lang="ts">
    import { busy } from '@duydang2311/svutils';
    import { onDestroy } from 'svelte';
    import { Loader } from '~/lib/components/icons';
    import { guardNull } from '~/lib/utils/guard';
    import ActivityItem from './ActivityItem.svelte';
    import AddComment from './AddComment.svelte';
    import { useActivityLists, usePageContext, useTask } from './utils.svelte';

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
</script>

<section class="mt-8 max-w-container-lg mx-auto">
    <div data-stuck={stuck ? '' : undefined} class="data-stuck:animate-pulse relative">
        {#if activities.length > 0}
            <ol class="flex flex-col gap-6 duration-300 animate-fade-in">
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
