<script lang="ts">
	import { portal } from '@zag-js/svelte';
	import { createMenu } from '~/lib/components/builders.svelte';
	import { button, menu } from '~/lib/utils/styles';

	const id = $props.id();
	const menuApi = createMenu({ id });
</script>

<div>
	<button
		{...menuApi.getTriggerProps()}
		class="{button({
			variant: 'base',
			icon: true,
			outlined: true,
			ghost: true,
		})} rounded-full p-0.5"
	>
		<img src="https://placehold.co/32" alt="you" class="size-avatar-sm rounded-full" />
	</button>
	<div use:portal {...menuApi.getPositionerProps()}>
		<ul {...menuApi.getContentProps()} class="{menu({ part: 'content' })} flex flex-col gap-1">
			{#each [{ href: '/sign-out', label: 'Log out' }] as item (item.href)}
				<li {...menuApi.getItemProps({ value: item.href })} class="group">
					<a
						href={item.href}
						class="{button({
							variant: 'base',
							ghost: true,
						})} group-[[data-highlighted]]:bg-base-active block"
					>
						{item.label}
					</a>
				</li>
			{/each}
		</ul>
	</div>
</div>
