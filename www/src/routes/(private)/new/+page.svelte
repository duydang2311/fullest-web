<script lang="ts">
    import { page } from '$app/state';
    import slug from 'slug';
    import Errors from '~/lib/components/Errors.svelte';
    import { PlusOutline } from '~/lib/components/icons';
    import { ErrorKind } from '~/lib/utils/errors';
    import { createForm } from '~/lib/utils/form.svelte';
    import { button, field, input } from '~/lib/utils/styles';
    import type { PageProps } from './$types';
    import { validator } from './_page/utils';

    const { data, form }: PageProps = $props();
    let summary = $state.raw<string>();
    let name = $state.raw<string>();
    let identifier = $derived(name ? slug(name) : null);
    const appForm = createForm({
        validator,
    }).syncErrors(() => (form?.kind === ErrorKind.Validation ? form.errors : undefined));
    const fieldErrors = $derived(appForm.errors ?? {});
</script>

<main class="bg-base-light dark:bg-base flex-1 p-4 md:p-8 lg:p-12">
    <form
        method="post"
        class="max-w-container-sm mx-auto flex-1 border border-base-border rounded-lg shadow-xs divide-y divide-base-border-weak"
        novalidate
        {@attach appForm.withEnhanced()}
    >
        <div class="p-4 flex justify-between gap-4">
            <div>
                <h1 class="text-title-xs text-base-fg-strong font-bold">Create a new project</h1>
                <p class="text-base-fg-dim text-body-sm mt-1">
                    Required fields are marked with an asterisk (*).
                </p>
            </div>
            <PlusOutline class="size-8 text-base-fg-strong" />
        </div>
        <div class="p-4">
            <fieldset class="space-y-4">
                <div class={field()}>
                    <label for="name" class="c-label">Project name *</label>
                    <input
                        id="name"
                        name="name"
                        type="text"
                        class={input()}
                        aria-invalid={fieldErrors.name != null}
                        bind:value={name}
                        required
                    />
                    <Errors
                        errors={fieldErrors.name}
                        transforms={{
                            required: 'Enter project name.',
                            string: 'Enter project name.',
                            min_length: (length: number) =>
                                `Project name must be at least ${length} characters long.`,
                            max_length: (length: number) =>
                                `Project name cannot exceed ${length} characters.`,
                            ERR_REQUIRED: 'Enter project name.',
                            ERR_MAX_LENGTH: 'Project name is too long.',
                            ERR_INVALID:
                                'Project name should only contain letters, numbers, spaces, hyphens, and underscores.',
                        }}
                        class="text-negative"
                    />
                    {#if identifier}
                        <p class="c-help-text wrap-anywhere">
                            Your project will be created at: <strong
                                >{page.url.origin}/{data.username}/{identifier}</strong
                            >.
                        </p>
                        <input type="hidden" name="identifier" value={identifier} />
                    {/if}
                </div>
                <div class={field()}>
                    <label for="summary" class="c-label">Description</label>
                    <input
                        id="summary"
                        name="summary"
                        type="text"
                        class={input()}
                        aria-invalid={fieldErrors.summary != null}
                        bind:value={summary}
                        maxlength="350"
                    />
                    <Errors
                        errors={fieldErrors.summary}
                        transforms={{
                            max_length: (length: number) =>
                                `Description cannot exceed ${length} characters.`,
                        }}
                        class="text-negative"
                    />
                    {#if summary && summary.length > 0}
                        <p class="c-help-text">
                            <strong>{summary.length}</strong> / 350 characters.
                        </p>
                    {/if}
                </div>
            </fieldset>
            {#if form && form.kind !== ErrorKind.Validation}
                <p class="c-help-text text-negative mt-4">
                    An error occurred while creating the project: {form.kind}.
                </p>
            {/if}
        </div>
        <div class="flex justify-end gap-4 p-4">
            <a
                href="/"
                class={button({
                    variant: 'base',
                    ghost: true,
                })}
            >
                <span>Cancel</span>
            </a>
            <button
                type="submit"
                class="{button({
                    variant: 'primary',
                    filled: true,
                })} flex items-center gap-2"
            >
                <PlusOutline />
                <span>Create</span>
            </button>
        </div>
    </form>
</main>
