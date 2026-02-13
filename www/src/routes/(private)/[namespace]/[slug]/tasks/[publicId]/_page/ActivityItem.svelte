<script lang="ts">
    import type { Snippet } from 'svelte';
    import { ActivityKind, type Activity } from '~/lib/models/activity';
    import type { User, UserPreset } from '~/lib/models/user';
    import AssignedActivity from './AssignedActivity.svelte';
    import CommentedActivity from './CommentedActivity.svelte';
    import CreatedActivity from './CreatedActivity.svelte';
    import PriorityChangedActivity from './PriorityChangedActivity.svelte';
    import StatusChangedActivity from './StatusChangedActivity.svelte';
    import UnassignedActivity from './UnassignedActivity.svelte';
    import { validators } from './utils.svelte';

    const {
        activity,
    }: {
        activity: Pick<Activity, 'createdTime' | 'kind' | 'data'> & {
            actor: Pick<User, 'id'> & UserPreset['Avatar'];
        };
    } = $props();
</script>

{#snippet li(children: Snippet)}
    <li class="relative">
        {@render children()}
        <div
            class="absolute w-px h-6 left-0 bottom-0 translate-y-full translate-x-5.5 bg-base-border"
        ></div>
    </li>
{/snippet}

{#if activity.kind === ActivityKind.Created}
    {#snippet children()}
        <CreatedActivity {activity} />
    {/snippet}
    {@render li(children)}
{:else if activity.kind === ActivityKind.Commented}
    {@const data = activity.data}
    {#if validators[activity.kind].check(data)}
        {#snippet children()}<CommentedActivity activity={{ ...activity, data }} />{/snippet}
        {@render li(children)}
    {/if}
{:else if activity.kind === ActivityKind.StatusChanged}
    {@const data = activity.data}
    {#if validators[activity.kind].check(data)}
        {#snippet children()}<StatusChangedActivity activity={{ ...activity, data }} />{/snippet}
        {@render li(children)}
    {/if}
{:else if activity.kind === ActivityKind.PriorityChanged}
    {@const data = activity.data}
    {#if validators[activity.kind].check(data)}
        {#snippet children()}<PriorityChangedActivity activity={{ ...activity, data }} />{/snippet}
        {@render li(children)}
    {/if}
{:else if activity.kind === ActivityKind.Assigned}
    {@const data = activity.data}
    {#if validators[activity.kind].check(data)}
        {#snippet children()}<AssignedActivity activity={{ ...activity, data }} />{/snippet}
        {@render li(children)}
    {/if}
{:else if activity.kind === ActivityKind.Unassigned}
    {@const data = activity.data}
    {#if validators[activity.kind].check(data)}
        {#snippet children()}<UnassignedActivity activity={{ ...activity, data }} />{/snippet}
        {@render li(children)}
    {/if}
{/if}
