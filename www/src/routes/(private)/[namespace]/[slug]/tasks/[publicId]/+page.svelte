<script lang="ts">
	import { createRef } from '@duydang2311/svutils';
	import CommentItem from './CommentItem.svelte';
	import CommentSection from './CommentSection.svelte';
	import Stats from './Stats.svelte';

	const { data, form } = $props();
	const commentList = createRef(() => data.streamedCommentList)
	const filteredItems = $derived(
		commentList.current?.items.filter((a) => a.id !== data.task.initialCommentId) ?? []
	);
</script>

<div
	class="divide-base-border-weak max-w-container-xl mx-auto w-full flex-1 *:py-4 lg:flex lg:divide-x"
>
	<div class="flex-1 *:px-8">
		<div class="hidden items-center gap-4 lg:flex">
			<h1>
				<span class="text-title-sm text-base-fg-strong font-bold">{data.task.title}</span>
				<span class="c-help-text inline-block align-text-top">#{data.task.publicId}</span>
			</h1>
		</div>
		<div class="lg:hidden">
			<div>
				<h1>
					<span class="text-title-sm text-base-fg-strong font-bold">{data.task.title}</span>
					<span class="c-help-text inline-block align-text-top">#{data.task.publicId}</span>
				</h1>
			</div>
			<div class="mt-4 flex flex-wrap gap-x-8 gap-y-2 *:min-w-40 lg:hidden">
				<Stats />
			</div>
		</div>
		<hr class="border-base-border-weak mt-4 w-full lg:hidden" />
		<div class="mt-4">
			<CommentItem comment={data.task.initialComment} />
		</div>
		<div class="mt-8">
			<CommentSection taskId={data.task.id} comments={filteredItems} />
		</div>
	</div>
	<div class="w-64 lg:pl-8">
		<div class="hidden flex-col gap-4 lg:flex">
			<Stats />
		</div>
	</div>
</div>
