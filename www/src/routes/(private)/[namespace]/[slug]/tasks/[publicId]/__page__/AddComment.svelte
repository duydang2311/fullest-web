<script lang="ts">
    import { createEditor, SlashCommandsExtension } from '~/lib/components/editor.svelte';
    import { IconSendOutline } from '~/lib/components/icons';
    import EditorSlashCommands from '~/lib/components/EditorSlashCommands.svelte';
    import { ActivityKind } from '~/lib/models/activity';
    import { guardNull } from '~/lib/utils/guard';
    import { usePageData } from '~/lib/utils/kit';
    import { button } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { addComment, getActivityList } from './page.remote';
    import { useActivityLists, usePageContext, useTask } from './utils.svelte';

    const data = usePageData<PageData>();
    const ctx = usePageContext();
    const activityLists = $derived(useActivityLists(ctx.activityListParams));
    const task = $derived(useTask());
    const editor = createEditor({
        placeholder: "Press '/' for commands...",
        editorProps: {
            attributes: {
                class: 'c-editor--inner prose max-w-none h-full flex-1',
            },
        },
        extensions: [SlashCommandsExtension],
    });

    async function handleSubmit() {
        guardNull(editor.current);
        guardNull(task.current);
        if (editor.current.isEmpty) {
            return;
        }
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
            taskId: task.current.id,
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

<div class="c-editor--container c-editor--outlined flex flex-col">
    <div class="flex-1 min-h-64 max-h-96 flex flex-col overflow-auto" {@attach editor}>
        {#if editor.current}
            <EditorSlashCommands editor={editor.current} />
        {/if}
    </div>
    <div class="sticky mt-4 flex justify-end gap-2 p-2">
        <button
            type="button"
            class={button({
                variant: 'primary',
                ghost: true,
                icon: true,
            })}
            onclick={handleSubmit}
        >
            <IconSendOutline />
        </button>
    </div>
</div>
