<script lang="ts">
	import { page } from '$app/state';
	import { MagnifyingGlass, PlusOutline } from '~/lib/components/icons';
	import { button } from '~/lib/utils/styles';
</script>

<div class="flex items-center justify-between gap-4">
	<div class="w-128 relative max-w-full">
		<input type="text" placeholder="Search tasks..." class="c-input w-full pl-8" />
		<MagnifyingGlass class="absolute left-2 top-1/2 -translate-y-1/2" />
	</div>
	<a
		href="/{page.params.namespace}/{page.params.slug}/tasks/new"
		class="{button({
			variant: 'primary',
			filled: true,
			outlined: true,
		})} flex items-center gap-2 text-nowrap"
	>
		<PlusOutline />
		New Task
	</a>
</div>
