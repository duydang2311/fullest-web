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

    const task = $derived(await useTask());
    const id = $props.id();
    const menu = createMenu({
        id,
    });
    const titleEdit = createInlineEdit();
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
                {#if titleEdit.enabled}
                    <form
                        {...editTaskTitle.enhance(async (e) => {
                            titleEdit.enabled = false;
                            await e.submit().updates(
                                useTask().withOverride((task) => ({
                                    ...task,
                                    title: e.data.title,
                                }))
                            );
                        })}
                        class="relative"
                    >
                        <input {...editTaskTitle.fields.taskId.as('text', task.id)} type="hidden" />
                        <input
                            {...editTaskTitle.fields.version.as('number', task.version)}
                            type="hidden"
                        />
                        <input
                            {...editTaskTitle.fields.title.as('text', task.title)}
                            class="text-title-sm font-semibold text-fg-emph w-full outline-none"
                            spellcheck="false"
                            {@attach titleEdit}
                        />
                        <div class="flex gap-2 mt-2 flex-wrap">
                            <button
                                type="button"
                                class={C.button({ size: 'sm', ghost: true })}
                                onclick={() => {
                                    titleEdit.enabled = false;
                                }}
                            >
                                Cancel
                            </button>
                            <button
                                type="submit"
                                class="{C.button({
                                    variant: 'primary',
                                    size: 'sm',
                                    filled: true,
                                })} flex gap-2 items-center"
                            >
                                <IconSaveOutline />
                                Save
                            </button>
                        </div>
                    </form>
                {:else}
                    <button
                        type="button"
                        {@attach titleEdit}
                        class="text-left text-fg-emph select-text cursor-text w-full"
                    >
                        <h1 class="text-title-sm w-fit">
                            <span>
                                {task.title}
                            </span>
                            <span
                                class="text-xs text-fg-muted px-1 rounded-xs font-normal align-middle"
                            >
                                #{task.publicId}
                            </span>
                        </h1>
                    </button>
                {/if}
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
