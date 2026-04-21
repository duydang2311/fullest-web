<script lang="ts">
    import type { InlineEdit } from '@duydang2311/sveltecraft';
    import { Extension } from '@tiptap/core';
    import DOMPurify from 'dompurify';
    import { untrack } from 'svelte';
    import { createEditor, renderToHTMLString } from '~/lib/components/editor.svelte';
    import { IconSaveOutline } from '~/lib/components/icons';
    import { guardNull } from '~/lib/utils/guard';
    import { C } from '~/lib/utils/styles';
    import { editTaskDescription } from './page.remote';
    import { useTask } from './utils.svelte';

    const { edit }: { edit: InlineEdit } = $props();
    const task = $derived(await useTask());
    const editor = createEditor({
        placeholder: 'Enter task description...',
        content: untrack(() => (task.descriptionJson ? JSON.parse(task.descriptionJson) : null)),
        editorProps: {
            attributes: {
                class: 'h-full focus-visible:outline-none prose max-w-none border border-base-border p-4 rounded-lg',
            },
        },
        extensions: [
            new Extension({
                name: 'custom-shortcuts',
                addKeyboardShortcuts() {
                    return {
                        'Ctrl-Enter': () => {
                            editDescription();
                            return true;
                        },
                        Escape: () => {
                            edit.enabled = false;
                            return true;
                        },
                    };
                },
            }),
        ],
        onCreate(props) {
            props.editor.commands.focus('end');
        },
    });

    async function editDescription() {
        guardNull(editor.current);
        const json = editor.current.getJSON();
        const jsonString = JSON.stringify(json);
        edit.enabled = false;
        await editTaskDescription({
            taskId: task.id,
            descriptionJson: jsonString,
            version: task.version,
        }).updates(
            useTask().withOverride((task) => ({
                ...task,
                descriptionJson: jsonString,
                descriptionHtml: DOMPurify.sanitize(renderToHTMLString(json)),
            }))
        );
    }
</script>

<div>
    <div {@attach editor}></div>
    <div class="flex gap-2 mt-2">
        <button
            type="button"
            class={C.button({ ghost: true, size: 'sm' })}
            onclick={() => {
                edit.enabled = false;
            }}>Cancel</button
        >
        <button
            type="button"
            class="{C.button({
                variant: 'primary',
                filled: true,
                size: 'sm',
            })} flex gap-2 items-center"
            onclick={editDescription}
        >
            <IconSaveOutline />
            <span>Save</span>
        </button>
    </div>
</div>
