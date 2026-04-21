<script lang="ts">
    import { createRef } from '@duydang2311/svutils';
    import { Editor } from '@tiptap/core';
    import { untrack } from 'svelte';
    import { createTextEditor } from '~/lib/components/editor.svelte';
    import { CheckOutline } from '~/lib/components/icons';
    import type { Comment } from '~/lib/models/comment';
    import { tsap } from '~/lib/utils/gsap';
    import { usePageState } from '~/lib/utils/kit';
    import { button } from '~/lib/utils/styles';
    import { editComment, getActivityList } from './page.remote';
    import type { PageState } from './types';

    let {
        taskId,
        comment,
        isEditing = $bindable(),
    }: {
        taskId: string;
        comment: Pick<Comment, 'id' | 'contentJson'>;
        isEditing: boolean;
    } = $props();
    const pageState = usePageState<PageState>();
    const editor = createRef<Editor>();
</script>

<div class="overflow-hidden">
    <div
        class="c-editor"
        {@attach (node) => {
            untrack(() => {
                editor.current = createTextEditor({
                    element: node,
                    content: comment?.contentJson ? JSON.parse(comment.contentJson) : undefined,
                    placeholder: 'Enter your comment...',
                    editorProps: {
                        attributes: {
                            class: 'c-editor--inner prose max-w-none p-0',
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
            onclick={() => {
                isEditing = false;
            }}
        >
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
                await editComment({
                    id: comment.id,
                    contentJson,
                }).updates(
                    getActivityList({
                        taskId,
                        cursor: pageState.cursor,
                    }).withOverride((a) => ({
                        ...a,
                        items: a.items.map((b) =>
                            b.id === comment.id ? { ...b, contentJson } : b
                        ),
                    }))
                );
                isEditing = false;
            }}
        >
            <CheckOutline />
            <span>Save</span>
        </button>
    </div>
</div>
