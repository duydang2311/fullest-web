<script lang="ts">
    import { invalidate } from '$app/navigation';
    import { page } from '$app/state';
    import { createRef } from '@duydang2311/svutils';
    import { Editor } from '@tiptap/core';
    import { untrack } from 'svelte';
    import { createTextEditor } from '~/lib/components/editor';
    import { CheckOutline, XOutline } from '~/lib/components/icons';
    import type { Comment } from '~/lib/models/comment';
    import { tsap } from '~/lib/utils/gsap';
    import { button } from '~/lib/utils/styles';
    import { editComment } from './page.remote';

    const {
        comment,
        onCancel,
        onSave,
    }: { comment?: Pick<Comment, 'id' | 'contentJson'>; onCancel: () => void; onSave: () => void } =
        $props();
    const editor = createRef<Editor>();
</script>

<div class="overflow-hidden c-editor--outlined">
    <div
        class="c-editor"
        {@attach (node) => {
            untrack(() => {
                editor.current = createTextEditor({
                    element: node,
                    content: comment?.contentJson ? JSON.parse(comment.contentJson) : undefined,
                    placeholder: 'Enter task description...',
                    editorProps: {
                        attributes: {
                            class: 'c-editor--inner prose max-w-none',
                        },
                    },
                    onTransaction: (props) => {
                        editor.current = null!;
                        editor.current = props.editor;
                    },
                });

                editor.current.commands.focus();
            });
        }}
    ></div>
    <div
        class="flex justify-end text-sm"
        in:tsap={(node, gsap) =>
            gsap.from(node, { yPercent: 100, opacity: 0, duration: 0.2, ease: 'sine.inOut' })}
    >
        <button
            type="button"
            class="{button({
                variant: 'base',
                filled: true,
                outlined: true,
            })} flex items-center gap-2 rounded-none rounded-tl-lg border-b-0"
            onclick={onCancel}
        >
            <XOutline />
            <span>Cancel</span>
        </button>
        <button
            type="button"
            class="{button({
                variant: 'primary',
                filled: true,
                outlined: true,
            })} flex items-center gap-2 rounded-none rounded-br-lg border-l-0 border-b-0 border-r-0"
            onclick={async () => {
                if (!comment || !editor.current) {
                    return;
                }

                const contentJson = JSON.stringify(editor.current.getJSON());
                comment.contentJson = contentJson;
                await editComment({
                    id: comment.id,
                    contentJson,
                });
                onSave();
                await invalidate(page.route.id!);
            }}
        >
            <CheckOutline />
            <span>Save</span>
        </button>
    </div>
</div>
