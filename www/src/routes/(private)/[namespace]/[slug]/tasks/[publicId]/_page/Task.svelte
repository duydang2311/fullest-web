<script lang="ts">
    import { portal } from '@zag-js/svelte';
    import sanitize from 'sanitize-html';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { renderToHTMLString } from '~/lib/components/editor';
    import { MenuOutline, PencilOutline, TrashOutline } from '~/lib/components/icons';
    import { button, C } from '~/lib/utils/styles';
    import { deleteTask } from './page.remote';
    import SelectAssignees from './SelectAssignees.svelte';
    import SelectPriority from './SelectPriority.svelte';
    import SelectStatus from './SelectStatus.svelte';
    import TaskEdit from './TaskEdit.svelte';
    import { usePageContext } from './utils.svelte';

    let isEditing = $state.raw(false);
    const ctx = usePageContext();
    const comment = $derived(ctx.task.initialComment);
    const id = $props.id();
    const menu = createMenu({
        id,
        onSelect: async (details) => {
            if (details.value === 'edit') {
                isEditing = true;
            }
        },
    });
</script>

<div>
    <div class="border-b border-b-surface-border pb-4 px-4 lg:px-8">
        <div class="flex justify-between items-center gap-8 max-w-container-lg mx-auto">
            <p class="c-help-text px-2 bg-surface-subtle border border-surface-border rounded-sm">
                #{ctx.task.publicId}
            </p>
            <div class="mt-4">
                <button
                    type="button"
                    class={button({ ghost: true, icon: true })}
                    {...menu.api.getTriggerProps()}
                >
                    <MenuOutline />
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
                                <input {...deleteTask.fields.id.as('hidden', ctx.task.id)} />
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
        <h1 class="leading-none text-title-sm text-fg-emph font-bold max-w-container-lg mx-auto">
            {ctx.task.title}
        </h1>
        <div class="mt-6 flex flex-wrap gap-x-2 gap-y-2 *:basis-40 *:flex-1 lg:hidden">
            <SelectStatus />
            <SelectPriority />
            <SelectAssignees />
        </div>
    </div>
    <div class="px-4 lg:px-8">
        {#if isEditing}
            <div class="mt-4 max-w-container-lg mx-auto">
                <TaskEdit
                    {comment}
                    onCancel={() => {
                        isEditing = false;
                    }}
                    onSave={() => {
                        isEditing = false;
                    }}
                />
            </div>
        {:else}
            <article class="prose wrap-anywhere mt-4 max-w-container-lg mx-auto">
                {#if comment?.contentJson && comment?.contentJson.length > 0}
                    <!-- eslint-disable-next-line svelte/no-at-html-tags -->
                    {@html sanitize(renderToHTMLString(JSON.parse(comment.contentJson)))}
                {:else}
                    <i class="text-fg-muted">No description provided.</i>
                {/if}
            </article>
        {/if}
    </div>
</div>
