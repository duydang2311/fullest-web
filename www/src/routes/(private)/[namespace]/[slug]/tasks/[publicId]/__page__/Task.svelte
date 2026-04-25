<script lang="ts">
    import { createInlineEdit } from '@duydang2311/sveltecraft';
    import { portal } from '@zag-js/svelte';
    import { createMenu } from '~/lib/components/builders.svelte';
    import {
        IconSaveOutline,
        PencilOutline,
        SettingsOutline,
        TrashOutline,
    } from '~/lib/components/icons';
    import { button, C } from '~/lib/utils/styles';
    import Breadcrumbs from '../../../__page__/Breadcrumbs.svelte';
    import EditTaskDescription from './EditTaskDescription.svelte';
    import { deleteTask, editTaskTitle } from './page.remote';
    import SelectAssignees from './SelectAssignees.svelte';
    import SelectPriority from './SelectPriority.svelte';
    import SelectStatus from './SelectStatus.svelte';
    import { useTask } from './utils.svelte';
    import TaskTitle from './TaskTitle.svelte';

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
</div>
