<script lang="ts">
    import { createRef } from '@duydang2311/svutils';
    import CommentItem from './CommentItem.svelte';
    import CommentSection from './CommentSection.svelte';
    import Stats from './Stats.svelte';
    import Task from './Task.svelte';

    const { data, form } = $props();
    const commentList = createRef(() => data.streamedCommentList);
    const filteredItems = $derived(
        commentList.current?.items.filter((a) => a.id !== data.task.initialCommentId) ?? []
    );
</script>

<div
    class="divide-base-border-weak max-w-container-xl mx-auto w-full flex-1 *:py-4 lg:flex lg:divide-x"
>
    <div class="flex-1 *:px-8">
        <div class="mt-4">
            <Task task={data.task} comment={data.task.initialComment} />
        </div>
        <div class="mt-8">
            <CommentSection user={data.user} taskId={data.task.id} comments={filteredItems} />
        </div>
    </div>
    <div class="w-64 lg:pl-8">
        <div class="hidden flex-col gap-4 lg:flex">
            <Stats />
        </div>
    </div>
</div>
