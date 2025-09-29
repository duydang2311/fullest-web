<script lang="ts">
	import { goto } from '$app/navigation';
	import { portal } from '@zag-js/svelte';
	import { createMenu } from '~/lib/components/builders.svelte';
	import { ChevronDownOutline, PlusOutline } from '~/lib/components/icons';
	import { button, menu } from '~/lib/utils/styles';

	const id = $props.id();
	const menuApi = createMenu({
		id,
		onSelect: async (details) => {
			await goto(details.value);
		},
	});
</script>

<button
	{...menuApi.getTriggerProps()}
	class="{button({
		variant: 'base',
		icon: true,
		outlined: true,
		filled: true,
	})} flex items-center gap-1"
>
	<PlusOutline />
	<ChevronDownOutline />
</button>
<div use:portal {...menuApi.getPositionerProps()}>
	<ul {...menuApi.getContentProps()} class="{menu({ part: 'content' })} flex flex-col gap-1">
		{#each [{ value: 'new-task', label: 'New task' }, { value: 'new-project', label: 'New project' }] as item (item.value)}
			<li {...menuApi.getItemProps({ value: item.value })} class="group">
				<button
					type="button"
					class="{button({
						variant: 'base',
						ghost: true,
					})} group-[[data-highlighted]]:bg-base-active w-full text-left"
				>
					{item.label}
				</button>
			</li>
		{/each}
	</ul>
</div>
