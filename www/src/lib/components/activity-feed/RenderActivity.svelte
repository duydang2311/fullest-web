<script lang="ts">
    import type { Component } from 'svelte';
    import { type Activity } from '~/lib/models/activity';
    import type { UserPreset } from '~/lib/models/user';
    import ActivityShell from './ActivityShell.svelte';
    import type { ActivityRender } from './utils.svelte';

    const {
        activity,
        renderer,
        shell,
    }: {
        activity: Pick<Activity, 'createdTime' | 'kind' | 'metadata'> & {
            actor: UserPreset['Avatar'];
        };
        renderer: ActivityRender<unknown>;
        shell?: Component;
    } = $props();
    const parsed = $derived(renderer.validator.parse(activity));
    const ComputedShell = $derived(shell ?? ActivityShell);
</script>

{#if parsed.ok}
    <li class="relative group">
        <ComputedShell {activity}>
            <renderer.component {activity} />
        </ComputedShell>
    </li>
{/if}
