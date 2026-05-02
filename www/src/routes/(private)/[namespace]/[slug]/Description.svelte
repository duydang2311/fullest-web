<script lang="ts">
    import { createInlineEdit } from '@duydang2311/sveltecraft';
    import DOMPurify from 'dompurify';
    import {
        createEditor,
        DisableInlineEditShortcutExtension,
        SubmitShortcutExtension,
    } from '~/lib/components/editor.svelte';
    import { IconDocumentTextOutline, IconSaveOutline } from '~/lib/components/icons';
    import { guardNull } from '~/lib/utils/guard';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from './$types';
    import { editDescription } from './__page__/page.remote';
    import { useProject } from './__page__/utils.svelte';

    const pageData = usePageData<PageData>();
    const edit = createInlineEdit();
    const project = $derived(await useProject());
    const editor = createEditor({
        placeholder: 'Write project description...',
        autofocus: 'end',
        get content() {
            return project.descriptionJson ? JSON.parse(project.descriptionJson) : null;
        },
        editorProps: {
            attributes: {
                class: 'c-editor-new prose max-w-none',
            },
        },
        extensions: [
            SubmitShortcutExtension.configure({
                onSubmit: submit,
            }),
            DisableInlineEditShortcutExtension.configure({ edit }),
        ],
    });

    async function submit() {
        guardNull(editor.current);
        const descriptionJson = editor.current.isEmpty
            ? null
            : JSON.stringify(editor.current.getJSON());
        const descriptionHtml = editor.current.isEmpty ? null : editor.current.getHTML();
        edit.enabled = false;
        await editDescription({
            projectId: pageData.project.id,
            descriptionJson,
            version: project.version,
        }).updates(
            useProject().withOverride((project) => ({
                ...project,
                descriptionJson,
                descriptionHtml: descriptionHtml ? DOMPurify.sanitize(descriptionHtml) : null,
            }))
        );
    }
</script>

<div>
    <div class="flex items-center mt-6 gap-2 text-fg-muted text-sm">
        <IconDocumentTextOutline />
        <h2>Description</h2>
    </div>
    <div class="mt-4">
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
        {:else if project.descriptionHtml}
            <article {@attach edit} class="prose max-w-none">
                {@html project.descriptionHtml}
            </article>
        {:else}
            <div {@attach edit} class="text-fg-muted text-sm -mt-2">No description yet.</div>
        {/if}
    </div>
</div>
