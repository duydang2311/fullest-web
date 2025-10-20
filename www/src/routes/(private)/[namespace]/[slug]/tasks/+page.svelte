<script lang="ts">
	import { page } from '$app/state';
	import { MagnifyingGlass, PlusOutline } from '~/lib/components/icons';
	import { button } from '~/lib/utils/styles';

	const { data } = $props();
</script>

<div class="max-w-container-xl mx-auto w-full px-8 py-4">
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
	<div class="mt-4">
		{#if data.taskList.items.length <= 0}
			<p class="c-help-text">No tasks found.</p>
		{:else}
			<div class="border-base-border overflow-auto rounded-lg border">
				<table
					class="bg-base-light min-w-container-md grid grid-cols-[1fr_auto_auto_auto] content-start text-left"
				>
					<thead class="col-span-full grid grid-cols-subgrid rounded-t-lg">
						<tr
							class="border-b-base-border-weak divide-base-border-weak col-span-full grid grid-cols-subgrid items-center divide-x border-b *:p-4"
						>
							<th>Task</th>
							<th>Status</th>
							<th>Priority</th>
							<th></th>
						</tr>
					</thead>
					<tbody class="divide-base-border-weak col-span-full grid grid-cols-subgrid divide-y">
						{#each data.taskList.items as task (task.id)}
							<tr
								class="divide-base-border-weak divide-x-base-border-weak col-span-full grid grid-cols-subgrid divide-x *:p-4 last:rounded-b-md last:*:last:rounded-br-md last:*:first:rounded-bl-md"
							>
								<td class="hover:bg-base-hover relative transition">
									<a
										href="/{page.params.namespace}/{page.params.slug}/tasks/{task.publicId}"
										aria-label="View task details"
										class="absolute inset-0"
									></a>
									<div class="text-base-fg-strong font-medium">
										{task.title}
									</div>
									<div class="c-help-text mt-1">
										#{task.publicId} · somebody opened 5 hours ago
									</div>
								</td>
								<td class="hover:bg-base-hover group relative min-w-48 font-medium transition">
									<span>Backlog</span>
									<div
										class="border-base-border border-3 absolute bottom-0 right-0 size-0 border-l-transparent border-t-transparent"
									></div>
								</td>
								<td class="hover:bg-base-hover relative min-w-48 font-medium transition">
									<span>Medium</span>
									<div
										class="border-base-border border-3 absolute bottom-0 right-0 size-0 border-l-transparent border-t-transparent"
									></div>
								</td>
								<td>Delete</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>
		{/if}
	</div>
</div>
