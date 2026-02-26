<script lang="ts">
    import type { Snippet } from 'svelte';
    import Avatar from '~/lib/components/Avatar.svelte';
    import type { UserPreset } from '~/lib/models/user';

    const {
        activity,
        children,
    }: {
        activity: {
            actor: UserPreset['Avatar'];
        };
        children: Snippet;
    } = $props();
</script>

<div class="relative">
    <div class="flex items-start gap-2">
        <a href="/{activity.actor.name}" class="shrink-0">
            <Avatar user={activity.actor} class="size-avatar-sm" />
        </a>
        <div class="flex-1">
            {@render children()}
        </div>
    </div>
    <div
        class="group-last:hidden absolute bg-surface-border h-full w-px bottom-4 translate-y-full left-3"
    ></div>
</div>
