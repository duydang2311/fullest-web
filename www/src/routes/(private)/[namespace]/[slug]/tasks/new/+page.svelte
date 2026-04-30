<script lang="ts">
    import { createEditor } from '~/lib/components/editor.svelte.js';
    import Errors from '~/lib/components/Errors.svelte';
    import { PlusOutline } from '~/lib/components/icons';
    import InlineAlert from '~/lib/components/InlineAlert.svelte';
    import { matchNone } from '~/lib/utils/comparer.js';
    import { flattenErrorEntries } from '~/lib/utils/errors.js';
    import { button, field, input, label } from '~/lib/utils/styles';
    import { createTask } from './__page__/page.remote.js';

    const { data } = $props();
    const editor = createEditor({
        editorProps: {
            attributes: {
                'aria-labelledby': 'description-label',
                class: `${input()} min-h-32 max-h-96 overflow-auto prose max-w-none`,
            },
        },
        placeholder: 'Enter task description',
        onTransaction(props) {
            editor.current = props.editor;
        },
        onBlur(props) {
            if (props.editor.isEmpty) {
                createTask.fields.descriptionJson.set(undefined);
            } else {
                createTask.fields.descriptionJson.set(JSON.stringify(editor.current.getJSON()));
            }
        },
    });
    const fieldErrors = $derived(
        createTask.result?.data.kind === 'HttpValidationError' ? createTask.result.data.errors : {}
    );
</script>

<div class="max-w-container-md mx-auto w-full px-8 py-4">
    <form {...createTask.enhance((e) => e.submit())}>
        <h1 class="text-title-xs tracking-tight">Create new task</h1>
        <p class="text-fg-muted text-sm">Required fields are marked with an asterisk (*).</p>
        <fieldset class="mt-4 space-y-4">
            <input {...createTask.fields.projectId.as('text', data.project.id)} type="hidden" />
            <input {...createTask.fields.descriptionJson.as('text')} type="hidden" />
            <div class={field()}>
                <label for="title" class={label()}>Title *</label>
                <input
                    {...createTask.fields.title.as('text')}
                    id="title"
                    type="text"
                    class={input()}
                    required
                    minlength="1"
                    placeholder="Enter task title"
                    aria-invalid={fieldErrors.title ? true : undefined}
                />
                <Errors
                    errors={fieldErrors.title}
                    transforms={{
                        required: 'Enter a title for the task.',
                        string: 'Enter a title for the task.',
                    }}
                    class="c-help-text text-negative"
                />
            </div>
            <div class={field()}>
                <!-- svelte-ignore a11y_label_has_associated_control -->
                <!-- svelte-ignore a11y_no_noninteractive_element_interactions -->
                <label
                    id="description-label"
                    class={label()}
                    onmousedown={() => {
                        editor.current?.commands.focus();
                    }}
                >
                    Description
                </label>
                <div {@attach editor}></div>
                <p class="c-help-text">Supported formats: Plain text, Markdown.</p>
            </div>
            <button
                type="submit"
                class="{button({
                    variant: 'primary',
                    filled: true,
                })} ml-auto flex items-center gap-2"
            >
                <PlusOutline />
                <span>Create Task</span>
            </button>
        </fieldset>
        {#if createTask.result}
            {@const res = createTask.result}
            {#if res.status === 403}
                <p class="text-sm text-negative">
                    You do not have permission to create tasks in this project.
                </p>
            {:else if res.data.kind === 'HttpValidationError'}
                {@const errors = Object.entries(res.data.errors)
                    .filter(([k]) => matchNone('title', 'description')(k))
                    .flatMap(flattenErrorEntries)}
                <InlineAlert variant="negative" header="Invalid submission">
                    <p class="c-help-text">Please review and correct the following errors:</p>
                    <Errors {errors} class="mt-1" />
                </InlineAlert>
            {/if}
        {/if}
    </form>
</div>
