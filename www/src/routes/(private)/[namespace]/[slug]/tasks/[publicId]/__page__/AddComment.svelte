<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import { createRef } from '@duydang2311/svutils';
    import { Editor } from '@tiptap/core';
    import { untrack } from 'svelte';
    import invariant from 'tiny-invariant';
    import { createTextEditor } from '~/lib/components/editor';
    import { PlusOutline } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import { usePageData } from '~/lib/utils/kit';
    import { button } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { addComment, getActivityList } from './page.remote';
    import { useActivityLists, usePageContext, useTask } from './utils.svelte';
    import { guardNull } from '~/lib/utils/guard';

    const editor = createRef<Editor>();
    const data = usePageData<PageData>();
    const ctx = usePageContext();
    const activityLists = $derived(useActivityLists(ctx.activityListParams));
    const task = $derived(await useTask());

    async function handleSubmit() {
        guardNull(editor.current);
        const lastList = activityLists.at(-1);
        guardNull(lastList);
        guardNull(lastList.query.current);

        const contentJson = editor.current.getJSON();
        editor.current.commands.clearContent();

        const optimisticActivity = {
            id: crypto.randomUUID(),
            kind: ActivityKind.Commented,
            createdTime: new Date().toISOString(),
            actor: data.user,
            metadata: {
                comment: {
                    id: crypto.randomUUID(),
                    contentJson: JSON.stringify(contentJson),
                },
            },
        };
        await addComment({
            contentJson: JSON.stringify(contentJson),
            taskId: task.id,
        }).updates(
            getActivityList(lastList.param).withOverride((list) => {
                return {
                    ...list,
                    items: [...list.items, optimisticActivity],
                };
            })
        );
    }
</script>

<div>
    <div
        class="flex-1 c-editor--container c-editor--outlined min-h-64 max-h-96 flex flex-col overflow-auto"
        {@attach (node) => {
            untrack(() => {
                node.style.display = '';
                editor.current = createTextEditor({
                    element: node,
                    placeholder: 'Add a comment...',
                    editorProps: {
                        attributes: {
                            class: 'c-editor--inner prose max-w-none h-full flex-1',
                        },
                    },
                    onTransaction: (props) => {
                        editor.current = null!;
                        editor.current = props.editor;
                    },
                });
            });
        }}
    ></div>
    <div class="mt-4 flex justify-end gap-2">
        <input type="hidden" name="taskId" value={task.id} />
        <button
            disabled={editor.current == null || editor.current.isEmpty}
            type="button"
            class="{button({
                variant: 'primary',
                filled: true,
            })} flex items-center gap-2"
            onclick={handleSubmit}
        >
            <PlusOutline />
            Add comment
        </button>
    </div>
</div>
