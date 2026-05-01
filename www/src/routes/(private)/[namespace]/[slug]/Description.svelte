<script lang="ts">
    import { createInlineEdit } from '@duydang2311/sveltecraft';
    import { createEditor, SubmitShortcutExtension } from '~/lib/components/editor.svelte';
    import { IconSaveOutline, SettingsOutline } from '~/lib/components/icons';
    import { guardNull } from '~/lib/utils/guard';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from './$types';
    import { editDescription } from './__page__/page.remote';

    const pageData = usePageData<PageData>();
    const edit = createInlineEdit();
    const editor = createEditor({
        placeholder: 'Write project description...',
        editorProps: {
            attributes: {
                class: 'c-editor-new prose max-w-none h-128',
            },
        },
        extensions: [
            SubmitShortcutExtension.configure({
                onSubmit: submit,
            }),
        ],
    });

    async function submit() {
        guardNull(editor.current);
        const descriptionJson = editor.current.isEmpty
            ? undefined
            : JSON.stringify(editor.current.getJSON());
        await editDescription({ projectId: pageData.project.id, descriptionJson });
    }
</script>

<div>
    <div class="flex items-center gap-4 justify-between mb-1">
        <h2 class="text-fg-emph tracking-tight flex-1">Description</h2>
        <button type="button" class={C.button({ ghost: true, size: 'sm', icon: true })}>
            <SettingsOutline />
        </button>
    </div>
    {#if edit.enabled}
        <div {@attach editor}></div>
        <div class="flex gap-2 mt-2">
            <button
                type="button"
                class={C.button({ ghost: true })}
                onclick={() => {
                    edit.enabled = false;
                }}
            >
                Cancel
            </button>
            <button
                type="button"
                class="{C.button({ variant: 'primary', filled: true })} flex items-center gap-2"
                onclick={submit}
            >
                <IconSaveOutline />
                <span>Save</span>
            </button>
        </div>
    {:else if pageData.project.descriptionHtml}
        <div {@attach edit}>
            {@html pageData.project.descriptionHtml}
        </div>
    {:else}
        <div {@attach edit} class="text-fg-muted text-sm">No description yet.</div>
    {/if}
</div>
