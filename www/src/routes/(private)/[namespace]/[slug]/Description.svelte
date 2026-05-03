<script lang="ts">
    import { portal } from '@zag-js/svelte';
    import DOMPurify from 'dompurify';
    import { createDialog, tooltip } from '~/lib/components/builders.svelte';
    import { createEditor, SubmitShortcutExtension } from '~/lib/components/editor.svelte';
    import {
        IconDocumentTextOutline,
        IconSaveOutline,
        SettingsOutline,
    } from '~/lib/components/icons';
    import { guardNull } from '~/lib/utils/guard';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from './$types';
    import { editDescription } from './__page__/page.remote';
    import { useProject } from './__page__/utils.svelte';

    const pageData = usePageData<PageData>();
    const project = $derived(await useProject());
    const editor = createEditor({
        placeholder: 'Write project description...',
        autofocus: 'end',
        get content() {
            return project.descriptionJson ? JSON.parse(project.descriptionJson) : null;
        },
        editorProps: {
            attributes: {
                class: 'c-editor-new border-none! prose max-w-none flex-1',
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
            ? null
            : JSON.stringify(editor.current.getJSON());
        const descriptionHtml = editor.current.isEmpty ? null : editor.current.getHTML();
        dialog.api.setOpen(false);
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
    const id = $props.id();
    let open = $state.raw(false);
    const dialog = createDialog({
        id,
        get open() {
            return open;
        },
        onOpenChange(details) {
            document.startViewTransition(() => {
                open = details.open;
            });
        },
    });
</script>

<div>
    <div class="flex items-center mt-6 gap-2 text-fg-emph text-sm">
        <IconDocumentTextOutline />
        <h2>Description</h2>
        <div class="ml-auto" {@attach tooltip('Edit description')}>
            <button
                type="button"
                class={C.button({ ghost: true, icon: true })}
                {...dialog.api.getTriggerProps()}
                style={dialog.api.open
                    ? 'visibility: hidden;'
                    : `view-transition-name: edit-desc-${id};`}
            >
                <SettingsOutline />
            </button>
        </div>
        {#if dialog.api.open}
            <div
                use:portal
                {...dialog.api.getBackdropProps()}
                class="fixed inset-0 bg-black/15 backdrop-blur-[1px]"
            ></div>
            <div
                use:portal
                {...dialog.api.getPositionerProps()}
                class="fixed top-0 left-0 flex justify-center items-center w-screen h-screen p-4"
            >
                <div
                    {...dialog.api.getContentProps()}
                    class="bg-surface rounded-xl border border-base-border w-full max-w-container-md flex flex-col h-full max-h-[80vh] overflow-hidden shadow-xs"
                >
                    <div class="p-4 bg-surface-subtle flex justify-between items-center">
                        <div>
                            <h2 {...dialog.api.getTitleProps()} class="text-title-xs text-fg-emph">
                                Edit description
                            </h2>
                            <p {...dialog.api.getDescriptionProps()} class="text-fg-muted text-sm">
                                Make changes to the project description below. Click save once you
                                are done.
                            </p>
                        </div>
                        <SettingsOutline
                            class="text-title-md text-fg-emph"
                            style="view-transition-name: edit-desc-{id};"
                        />
                    </div>
                    <hr class="border-surface-border" />
                    <div {@attach editor} class="contents"></div>
                    <div class="flex gap-2 justify-end p-4">
                        <button
                            type="button"
                            class={C.button({ ghost: true })}
                            {...dialog.api.getCloseTriggerProps()}
                        >
                            Cancel
                        </button>
                        <button
                            type="button"
                            class="{C.button({
                                variant: 'primary',
                                filled: true,
                            })} flex items-center gap-2"
                            onclick={submit}
                        >
                            <IconSaveOutline />
                            <span>Save</span>
                        </button>
                    </div>
                </div>
            </div>
        {/if}
    </div>
    <div class="mt-4">
        {#if project.descriptionHtml}
            <article class="prose max-w-none">
                {@html project.descriptionHtml}
            </article>
        {:else}
            <div class="text-fg-muted text-sm -mt-2">No description yet.</div>
        {/if}
    </div>
</div>
