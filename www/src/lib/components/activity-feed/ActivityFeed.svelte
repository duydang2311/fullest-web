<script lang="ts" module>
    import { ActivityKind } from '~/lib/models/activity';
    import ActivityAssigned from './ActivityAssigned.svelte';
    import ActivityCommented from './ActivityCommented.svelte';
    import ActivityCreated from './ActivityCreated.svelte';
    import ActivityPriorityChanged from './ActivityPriorityChanged.svelte';
    import ActivityStatusChanged from './ActivityStatusChanged.svelte';
    import ActivityTitleChanged from './ActivityTitleChanged.svelte';
    import ActivityUnassigned from './ActivityUnassigned.svelte';
    import { activityValidators, type ActivityRender } from './utils.svelte';

    const defaultRenderers = {
        [ActivityKind.Created]: {
            validator: activityValidators[ActivityKind.Created],
            component: ActivityCreated,
        },
        [ActivityKind.Commented]: {
            validator: activityValidators[ActivityKind.Commented],
            component: ActivityCommented,
        },
        [ActivityKind.Assigned]: {
            validator: activityValidators[ActivityKind.Assigned],
            component: ActivityAssigned,
        },
        [ActivityKind.Unassigned]: {
            validator: activityValidators[ActivityKind.Unassigned],
            component: ActivityUnassigned,
        },
        [ActivityKind.StatusChanged]: {
            validator: activityValidators[ActivityKind.StatusChanged],
            component: ActivityStatusChanged,
        },
        [ActivityKind.PriorityChanged]: {
            validator: activityValidators[ActivityKind.PriorityChanged],
            component: ActivityPriorityChanged,
        },
        [ActivityKind.TitleChanged]: {
            validator: activityValidators[ActivityKind.TitleChanged],
            component: ActivityTitleChanged,
        },
    } as const;
</script>

<script lang="ts">
    import type { Component, Snippet } from 'svelte';
    import type { HTMLAttributes } from 'svelte/elements';
    import { type Activity } from '~/lib/models/activity';
    import type { UserPreset } from '~/lib/models/user';
    import RenderActivity from './RenderActivity.svelte';

    const {
        activities,
        renderers,
        shell,
        children,
        ...props
    }: {
        activities: (Pick<Activity, 'id' | 'createdTime' | 'kind' | 'metadata'> & {
            actor: UserPreset['Avatar'];
        })[];
        renderers?: Partial<Record<ActivityKind, ActivityRender<any>>>;
        shell?: Component<any>;
        children?: Snippet;
    } & HTMLAttributes<HTMLElement> = $props();
    const mergedRenders = $derived({
        ...defaultRenderers,
        ...renderers,
    });
</script>

<ol {...props}>
    {@render children?.()}
    {#each activities as activity (activity.id)}
        {@const renderer = mergedRenders[activity.kind]}
        {#if renderer}
            <RenderActivity {activity} renderer={renderer as any} {shell} />
        {/if}
    {/each}
</ol>
