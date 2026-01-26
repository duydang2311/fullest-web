<script lang="ts">
    import { createRef } from '@duydang2311/svutils';
    import { Editor } from '@tiptap/core';
    import { untrack } from 'svelte';
    import invariant from 'tiny-invariant';
    import { createTextEditor } from '~/lib/components/editor';
    import { PlusOutline } from '~/lib/components/icons';
    import type { UserPreset } from '~/lib/models/user';
    import { button } from '~/lib/utils/styles';
    import { addComment, getActivities } from './page.remote';
    import { ActivityKind } from '~/lib/models/activity';

    const { user, taskId }: { user: UserPreset['Avatar']; taskId: string } = $props();
    const editor = createRef<Editor>();

    async function handleSubmit() {
        invariant(editor.current, 'editor must not be null');

        const contentJson = editor.current.getJSON();
        await addComment({
            taskId,
            contentJson,
        }).updates(
            getActivities(taskId).withOverride((a) => ({
                ...a,
                items: [
                    ...a.items,
                    {
                        id: crypto.randomUUID(),
                        kind: ActivityKind.Commented,
                        createdTime: new Date().toISOString(),
                        actor: user,
                        data: {
                            comment: {
                                contentJson: JSON.stringify(contentJson),
                            },
                        },
                    },
                ],
            }))
        );

        editor.current.commands.clearContent();
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
        <input type="hidden" name="taskId" value={taskId} />
        <button
            disabled={editor.current == null || editor.current.isEmpty}
            type="button"
            class="{button({
                variant: 'primary',
                outlined: true,
                filled: true,
            })} flex items-center gap-2"
            onclick={handleSubmit}
        >
            <PlusOutline />
            Add comment
        </button>
    </div>
</div>
