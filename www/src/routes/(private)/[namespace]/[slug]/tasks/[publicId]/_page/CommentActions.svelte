<script lang="ts">
    import { invalidateAll } from '$app/navigation';
    import invariant from 'tiny-invariant';
    import { MenuOutline, PencilOutline, TrashOutline } from '~/lib/components/icons';
    import type { Comment } from '~/lib/models/comment';
    import { button } from '~/lib/utils/styles';
    import { deleteComment } from './page.remote';
    import { usePageContext, validators } from './utils.svelte';

    let { comment, isEditing = $bindable() }: { comment: Pick<Comment, 'id'>; isEditing: boolean } =
        $props();
    const ctx = usePageContext();
</script>

<div class="flex gap-2">
    <button
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
    </button>
</div>
