<script lang="ts">
    import { portal } from '@zag-js/svelte';
    import DOMPurify from 'isomorphic-dompurify';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { renderToHTMLString } from '~/lib/components/editor';
    import { MenuOutline, PencilOutline, TrashOutline } from '~/lib/components/icons';
    import type { Comment } from '~/lib/models/comment';
    import type { Task } from '~/lib/models/task';
    import type { User } from '~/lib/models/user';
    import { button, C } from '~/lib/utils/styles';
    import { deleteTask } from './page.remote';
    import Stats from './Stats.svelte';
    import TaskEdit from './TaskEdit.svelte';

    const {
        task,
        comment,
    }: {
        task: Pick<Task, 'id' | 'title' | 'publicId'> & {
            author: Pick<User, 'name' | 'displayName'>;
        };
        comment: Pick<Comment, 'id' | 'contentJson'> & {
            author: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
        };
    } = $props();
    let isEditing = $state.raw(false);
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
    <div class="flex justify-between items-center gap-8">
        <p class="c-help-text">#{task.publicId}</p>
        <div>
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
    <h1 class="text-title-sm text-base-fg-strong font-bold">
        {task.title}
    </h1>
    <div class="mt-4 flex flex-wrap gap-x-8 gap-y-2 *:min-w-40 lg:hidden">
        <Stats />
    </div>
    {#if isEditing}
        <div class="mt-4">
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
        <hr class="border-base-border-weak mt-4 w-full" />
        <article class="prose max-w-none wrap-anywhere mt-4">
            {#if comment?.contentJson && comment?.contentJson.length > 0}
                {@html DOMPurify.sanitize(renderToHTMLString(JSON.parse(comment.contentJson)))}
            {:else}
                <i class="text-base-fg-muted">No description provided.</i>
            {/if}
        </article>
    {/if}
</div>
