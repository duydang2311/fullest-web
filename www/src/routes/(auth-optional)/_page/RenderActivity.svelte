<script lang="ts" module>
    import ActivityAssigned from './ActivityAssigned.svelte';
    import ActivityCommented from './ActivityCommented.svelte';
    import ActivityCreated from './ActivityCreated.svelte';
    import ActivityPriorityChanged from './ActivityPriorityChanged.svelte';
    import ActivityStatusChanged from './ActivityStatusChanged.svelte';
    import ActivityUnassigned from './ActivityUnassigned.svelte';
    import ActivityTitleChanged from './ActivityTitleChanged.svelte';
    import { activityValidators } from './utils.svelte';

    const renderers = {
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
    import { ActivityKind, type Activity } from '~/lib/models/activity';
    import type { UserPreset } from '~/lib/models/user';
    import CrossProjectActivityShell from './CrossProjectActivityShell.svelte';

    const {
        activity,
    }: {
        activity: Pick<Activity, 'createdTime' | 'kind' | 'metadata'> & {
            actor: UserPreset['Avatar'];
        };
    } = $props();
    const renderer = $derived(renderers[activity.kind as keyof typeof renderers]);
</script>

{#if renderer && renderer.validator.check(activity)}
    <li class="group">
        <CrossProjectActivityShell {activity}>
            <!-- eslint-disable-next-line @typescript-eslint/no-explicit-any -->
            <renderer.component activity={activity as any} />
        </CrossProjectActivityShell>
    </li>
{/if}
