<script lang="ts">
    import { MenuOutline, PencilOutline, TrashOutline } from '~/lib/components/icons';
    import type { Comment } from '~/lib/models/comment';
    import { button } from '~/lib/utils/styles';
    import { deleteComment, getActivities } from './page.remote';

    let {
        taskId,
        comment,
        isEditing = $bindable(),
    }: { taskId: string; comment: Pick<Comment, 'id'>; isEditing: boolean } = $props();
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
                await deleteComment({ id: comment.id }).updates(
                    getActivities(taskId).withOverride((a) => ({
                        ...a,
                        items: a.items.filter((b) => b.id !== comment.id),
                    }))
                );
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
