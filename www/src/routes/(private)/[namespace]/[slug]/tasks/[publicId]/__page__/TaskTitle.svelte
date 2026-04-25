<script lang="ts">
    import { createInlineEdit } from '@duydang2311/sveltecraft';
    import { IconSaveOutline } from '~/lib/components/icons';
    import { ActivityKind } from '~/lib/models/activity';
    import { guardNull } from '~/lib/utils/guard';
    import { usePageData } from '~/lib/utils/kit';
    import { C } from '~/lib/utils/styles';
    import type { PageData } from '../$types';
    import { editTaskTitle, getActivityList } from './page.remote';
    import { useActivityLists, usePageContext, useTask } from './utils.svelte';

    const ctx = usePageContext();
    const activityLists = $derived(useActivityLists(ctx.activityListParams));
    const task = $derived(await useTask());
    const edit = createInlineEdit();
    const pageData = usePageData<PageData>();
</script>

{#if edit.enabled}
    <form
        {...editTaskTitle.enhance(async (e) => {
            const lastList = activityLists.at(-1);
            guardNull(lastList);
            guardNull(lastList.query.current);
            edit.enabled = false;
            const optimisticActivity = {
                id: self.crypto.randomUUID(),
                actor: pageData.user,
                createdTime: new Date().toISOString(),
                kind: ActivityKind.TitleChanged,
                metadata: {
                    title: e.data.title,
                    oldTitle: task.title,
                },
            };
            await e.submit().updates(
                useTask().withOverride((task) => ({
                    ...task,
                    title: e.data.title,
                })),
                getActivityList(lastList.param).withOverride((list) => {
                    return {
                        ...list,
                        items: [...list.items, optimisticActivity],
                    };
                })
            );
        })}
        class="relative"
    >
        <input {...editTaskTitle.fields.taskId.as('text', task.id)} type="hidden" />
        <input {...editTaskTitle.fields.version.as('number', task.version)} type="hidden" />
        <input
            {...editTaskTitle.fields.title.as('text', task.title)}
            class="text-title-sm font-semibold text-fg-emph w-full outline-none"
            spellcheck="false"
            {@attach edit}
        />
        <div class="flex gap-2 mt-2 flex-wrap">
            <button
                type="button"
                class={C.button({ size: 'sm', ghost: true })}
                onclick={() => {
                    edit.enabled = false;
                }}
            >
                Cancel
            </button>
            <button
                type="submit"
                class="{C.button({
                    variant: 'primary',
                    size: 'sm',
                    filled: true,
                })} flex gap-2 items-center"
            >
                <IconSaveOutline />
                Save
            </button>
        </div>
    </form>
{:else}
    <button
        type="button"
        {@attach edit}
        class="text-left text-fg-emph select-text cursor-text w-full"
    >
        <h1 class="text-title-sm w-fit">
            <span>
                {task.title}
            </span>
            <span class="text-xs text-fg-muted px-1 rounded-xs font-normal align-middle">
                #{task.publicId}
            </span>
        </h1>
    </button>
{/if}
