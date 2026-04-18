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
    import { addComment } from './page.remote';
    import { usePageContext, useTask } from './utils.svelte';

    const editor = createRef<Editor>();
    const data = usePageData<PageData>();
    const task = $derived(await useTask());
    const ctx = usePageContext();

    async function handleSubmit() {
        invariant(editor.current, 'editor must not be null');

        const contentJson = editor.current.getJSON();
        editor.current.commands.clearContent();
        const oldActivityList = ctx.activityList;
        invariant(oldActivityList, 'oldActivityList must not be null');
        ctx.activityList = {
            ...oldActivityList,
            items: [
                ...oldActivityList.items,
                {
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
                },
            ],
        };

        await addComment({
            contentJson,
            taskId: task.id,
        }).finally(invalidateAll);
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
