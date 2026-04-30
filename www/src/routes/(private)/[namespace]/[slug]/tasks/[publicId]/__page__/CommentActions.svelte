<script lang="ts">
    import type { RemoteQueryUpdate } from '@sveltejs/kit';
    import { portal } from '@zag-js/svelte';
    import { isPlainObject } from 'is-what';
    import { createMenu } from '~/lib/components/builders.svelte';
    import { MenuOutline, PencilOutline, TrashOutline } from '~/lib/components/icons';
    import { ActivityKind, type Activity } from '~/lib/models/activity';
    import type { Comment } from '~/lib/models/comment';
    import { guardNull } from '~/lib/utils/guard';
    import { button, C } from '~/lib/utils/styles';
    import { deleteComment, getActivityList } from './page.remote';
    import { useActivityLists, usePageContext } from './utils.svelte';

    let { comment, isEditing = $bindable() }: { comment: Pick<Comment, 'id'>; isEditing: boolean } =
        $props();
    const ctx = usePageContext();
    const id = $props.id();
    const activityLists = $derived(useActivityLists(ctx.activityListParams));
    const menu = createMenu({
        id,
        async onSelect(details) {
            if (details.value === 'delete') {
                function matchActivity(a: Pick<Activity, 'kind' | 'metadata'>) {
                    return (
                        a.kind === ActivityKind.Commented &&
                        isPlainObject(a.metadata) &&
                        'comment' in a.metadata &&
                        isPlainObject(a.metadata.comment) &&
                        'id' in a.metadata.comment &&
                        a.metadata.comment.id &&
                        typeof a.metadata.comment.id === 'string' &&
                        a.metadata.comment.id === comment.id
                    );
                }
                const list = activityLists.find((list) => {
                    return list.query.current?.items.some(matchActivity);
                });

                let updates: RemoteQueryUpdate[] = [];
                if (list) {
                    guardNull(list.query.current);
                    updates.push(
                        list.query.withOverride((list) => ({
                            ...list,
                            items: list.items.filter((a) => !matchActivity(a)),
                        }))
                    );
                }
                const result = await deleteComment({ id: comment.id }).updates(...updates);
                if (result.ok) {
                    if (list) {
                        getActivityList({
                            ...list.param,
                            size: list.param.size - 1,
                        }).set(list.query.current!);
                        list.param.size -= 1;
                    }
                }
            }
        },
    });
</script>

<div class="flex gap-2">
    <button
        {...menu.api.getTriggerProps()}
        type="button"
        class={button({
            variant: 'base',
            icon: true,
            ghost: true,
            size: 'sm',
        })}
        title="Comment actions"
    >
        <MenuOutline />
    </button>
    <div use:portal {...menu.api.getPositionerProps()}>
        <ul {...menu.api.getContentProps()} class="{C.menu({ part: 'content' })} min-w-32">
            <li>
                <button
                    {...menu.api.getItemProps({ value: 'edit' })}
                    type="button"
                    class="{C.menu({ part: 'item' })} flex items-center gap-2 w-full"
                >
                    <PencilOutline {...menu.api.getItemIndicatorProps({ value: 'edit' })} />
                    <span>Edit</span>
                </button>
            </li>
            <li>
                <button
                    type="button"
                    {...menu.api.getItemProps({ value: 'delete' })}
                    class="{C.menu({
                        part: 'item',
                        variant: 'negative',
                    })} flex items-center gap-2 w-full"
                >
                    <TrashOutline />
                    <span>Delete</span>
                </button>
            </li>
        </ul>
    </div>
    <!-- <button
        type="button"
        disabled={isEditing}
        class="{button({
            variant: 'base',
            icon: true,
            ghost: true,
        })} hidden lg:block"
        title="Edit"
        onclick={() => {
            isEditing = true;
        }}
    >
        <PencilOutline />
    </button>
    {#if comment}
        <button
            type="button"
            class="{button({
                variant: 'negative',
                icon: true,
                ghost: true,
            })} hidden lg:block"
            title="Delete"
            onclick={async () => {
                const oldActivityList = ctx.activityList;
                invariant(oldActivityList, 'oldActivityList must not be null');
                ctx.activityList = {
                    ...oldActivityList,
                    items: oldActivityList.items.filter(
                        (a) =>
                            !validators.commented.check(a.metadata) ||
                            a.metadata.comment.id !== comment.id
                    ),
                };
                await deleteComment({ id: comment.id }).finally(invalidateAll);
            }}
        >
            <TrashOutline />
        </button>
    {/if}
    <button
        type="button"
        class="{button({
            variant: 'base',
            icon: true,
            ghost: true,
        })} lg:hidden"
        title="Open actions menu"
    >
        <MenuOutline />
    </button> -->
</div>
