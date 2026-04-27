<script lang="ts">
    import { createInlineEdit } from '@duydang2311/sveltecraft';
    import { portal } from '@zag-js/svelte';
    import Avatar from '~/lib/components/Avatar.svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import {
        IconCalendarOutline,
        PencilOutline,
        SettingsOutline,
        TrashOutline,
    } from '~/lib/components/icons';
    import { formatDate } from '~/lib/utils/date';
    import { button, C } from '~/lib/utils/styles';
    import Breadcrumbs from '../../../__page__/Breadcrumbs.svelte';
    import EditTaskDescription from './EditTaskDescription.svelte';
    import { deleteTask } from './page.remote';
    import SelectAssignees from './SelectAssignees.svelte';
    import SelectPriority from './SelectPriority.svelte';
    import SelectStatus from './SelectStatus.svelte';
    import TaskTitle from './TaskTitle.svelte';
    import { useTask } from './utils.svelte';
    import { namespaceUrl } from '~/lib/utils/url';

    const task = $derived(await useTask());
    const id = $props.id();
    const menu = createMenu({
        id,
    });
    const descEdit = createInlineEdit();
</script>

<div>
    <div class="px-8">
        <div class="max-w-container-lg mx-auto">
            <div class="flex justify-between gap-8 items-center">
                <Breadcrumbs />
                <div>
                    <button
                        type="button"
                        class={button({ ghost: true, icon: true, size: 'sm' })}
                        {...menu.api.getTriggerProps()}
                    >
                        <SettingsOutline />
                    </button>
                    <div use:portal {...menu.api.getPositionerProps()}>
                        <ul {...menu.api.getContentProps()} class={C.menu({ part: 'content' })}>
                            <li>
                                <button
                                    {...menu.api.getItemProps({ value: 'edit' })}
                                    type="submit"
                                    class="{C.menu({
                                        part: 'item',
                                    })} w-full px-2 flex items-center gap-4"
                                >
                                    <PencilOutline />
                                    Edit
                                </button>
                            </li>
                            <li>
                                <form {...deleteTask}>
                                    <input {...deleteTask.fields.id.as('hidden', task.id)} />
                                    <button
                                        type="submit"
                                        {...menu.api.getItemProps({ value: 'delete' })}
                                        class="{C.menu({
                                            part: 'item',
                                            variant: 'negative',
                                        })} w-full px-2 flex items-center gap-4"
                                    >
                                        <TrashOutline />
                                        Delete
                                    </button>
                                </form>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="mt-1">
                <TaskTitle />
            </div>
            <div class="mt-4 flex flex-wrap gap-x-2 gap-y-2 *:basis-40 *:flex-1 lg:hidden">
                <SelectStatus />
                <SelectPriority />
                <SelectAssignees />
            </div>
        </div>
    </div>
    <div class="px-8 mt-4">
        <div class="max-w-container-lg mx-auto">
            {#if descEdit.enabled}
                <EditTaskDescription edit={descEdit} />
            {:else}
                <article class="max-w-none prose wrap-anywhere" {@attach descEdit}>
                    {#if task.descriptionHtml && task.descriptionHtml.length > 0}
                        {@html task.descriptionHtml}
                    {:else}
                        <span class="text-fg-muted text-sm">No description added.</span>
                    {/if}
                </article>
            {/if}
        </div>
    </div>
    <div class="px-8 mt-8">
        <div class="max-w-container-lg mx-auto flex items-center gap-2 text-sm">
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
</div>
