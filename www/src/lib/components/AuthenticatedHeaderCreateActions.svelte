<script lang="ts">
	import { goto } from '$app/navigation';
	import { portal } from '@zag-js/svelte';
	import { createMenu } from '~/lib/components/builders.svelte';
	import {
		ChevronDownOutline,
		PlusOutline,
		ProjectOutline,
		TaskOutline,
	} from '~/lib/components/icons';
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
		{#each [{ icon: ProjectOutline, href: '/new', label: 'New project' }, { icon: TaskOutline, href: '/new-task', label: 'New task' }] as item (item.href)}
			<li>
				<a
					href={item.href}
					{...menuApi.getItemProps({ value: item.href })}
					class="{menu({ part: 'item' })} flex items-center gap-4"
				>
					<item.icon />
					<span>
						{item.label}
					</span>
				</a>
			</li>
		{/each}
	</ul>
</div>
