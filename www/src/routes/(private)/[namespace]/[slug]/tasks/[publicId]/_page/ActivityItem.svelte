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
        activity: Pick<Activity, 'createdTime' | 'kind' | 'metadata'> & {
            actor: Pick<User, 'id'> & UserPreset['Avatar'];
        };
    } = $props();
</script>

{#snippet li(children: Snippet)}
    <li class="relative">
        {@render children()}
        <div
            class="absolute w-px h-full left-0 bottom-0 translate-y-full translate-x-4 bg-base-border"
        ></div>
    </li>
{/snippet}

{#if activity.kind === ActivityKind.Created}
    {#snippet children()}
        <CreatedActivity {activity} />
    {/snippet}
    {@render li(children)}
{:else if activity.kind === ActivityKind.Commented}
    {@const metadata = activity.metadata}
    {#if validators[activity.kind].check(metadata)}
        {#snippet children()}<CommentedActivity activity={{ ...activity, metadata }} />{/snippet}
        {@render li(children)}
    {/if}
{:else if activity.kind === ActivityKind.StatusChanged}
    {@const metadata = activity.metadata}
    {#if validators[activity.kind].check(metadata)}
        {#snippet children()}
            <StatusChangedActivity activity={{ ...activity, metadata }} />
        {/snippet}
        {@render li(children)}
    {/if}
{:else if activity.kind === ActivityKind.PriorityChanged}
    {@const metadata = activity.metadata}
    {#if validators[activity.kind].check(metadata)}
        {#snippet children()}
            <PriorityChangedActivity activity={{ ...activity, metadata }} />
        {/snippet}
        {@render li(children)}
    {/if}
{:else if activity.kind === ActivityKind.Assigned}
    {@const metadata = activity.metadata}
    {#if validators[activity.kind].check(metadata)}
        {#snippet children()}<AssignedActivity activity={{ ...activity, metadata }} />{/snippet}
        {@render li(children)}
    {/if}
{:else if activity.kind === ActivityKind.Unassigned}
    {@const metadata = activity.metadata}
    {#if validators[activity.kind].check(metadata)}
        {#snippet children()}<UnassignedActivity activity={{ ...activity, metadata }} />{/snippet}
        {@render li(children)}
    {/if}
{/if}
