<script lang="ts">
    import Avatar from '~/lib/components/Avatar.svelte';
    import Card from '~/lib/components/Card.svelte';
    import { IconCalendarOutline, SettingsOutline } from '~/lib/components/icons';
    import { formatDate } from '~/lib/utils/date';
    import { C } from '~/lib/utils/styles';
    import { namespaceUrl } from '~/lib/utils/url';
    import SelectAssignees from './SelectAssignees.svelte';
    import SelectPriority from './SelectPriority.svelte';
    import SelectStatus from './SelectStatus.svelte';
    import TaskTitle from './TaskTitle.svelte';
    import { useTask } from './utils.svelte';

    const task = $derived(await useTask());
</script>

<div>
    <TaskTitle />
    <div class="mt-2">
        <div class="flex items-center gap-2 text-sm">
            <div class="flex items-center gap-2">
                <a
                    href={namespaceUrl(task.author.name)}
                    aria-label="Navigate to {task.author.name}'s profile"
                >
                    <Avatar user={task.author} class="size-avatar-xs" />
                </a>
                <div>
                    <span class="text-fg-dim"> Created by </span>
                    <a
                        href={namespaceUrl(task.author.name)}
                        aria-label="Navigate to {task.author.name}'s profile"
                        class="c-link"
                    >
                        <span class="font-medium">
                            {task.author.displayName ?? task.author.name}
                        </span>
                    </a>
                </div>
            </div>
            <span class="text-fg-muted">·</span>
            <div class="flex items-center gap-2">
                <IconCalendarOutline class="size-avatar-xs" />
                <span class="text-fg-dim">{formatDate(task.createdTime)}</span>
            </div>
            <span class="text-fg-muted">·</span>
            <div class="flex items-center gap-2">
                <span class="text-fg-dim">#{task.publicId}</span>
            </div>
        </div>
    </div>
    <div class="mt-2 flex flex-wrap gap-x-2 gap-y-2 *:basis-40 *:flex-1 lg:hidden">
        <SelectStatus />
        <SelectPriority />
        <SelectAssignees />
    </div>
    <Card class="mt-4">
        {#snippet header()}
            <div class="flex justify-between gap-4 items-center">
                <span class="text-sm text-fg-emph font-semibold">Description</span>
                <button type="button" class={C.button({ ghost: true, icon: true })}>
                    <SettingsOutline />
                </button>
            </div>
        {/snippet}
        {#snippet body()}
            <div class="max-w-none prose wrap-anywhere">
                {#if task.descriptionHtml && task.descriptionHtml.length > 0}
                    {@html task.descriptionHtml}
                {:else}
                    <span class="text-fg-muted text-sm">No description added.</span>
                {/if}
            </div>
        {/snippet}
    </Card>
</div>
