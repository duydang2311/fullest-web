<script lang="ts">
	import { goto } from '$app/navigation';
	import { portal } from '@zag-js/svelte';
	import { createMenu } from '~/lib/components/builders.svelte';
	import { MenuOutline } from '~/lib/components/icons';
	import { button } from '~/lib/utils/styles';

	const id = $props.id();
	const menuApi = createMenu({
		id,
		onSelect: async (details) => {
			await goto(details.value);
		},
	});
</script>

<div class="hidden items-center gap-2 sm:flex">
	<a href="/sign-in" class={button({ variant: 'base', ghost: true })}> Sign in </a>
	<a href="/sign-up" class={button({ variant: 'primary', filled: true })}> Sign up </a>
</div>
<div class="sm:hidden">
	<button
		{...menuApi.getTriggerProps()}
		class={button({ variant: 'base', icon: true, ghost: true })}
	>
		<MenuOutline />
	</button>
	<div use:portal {...menuApi.getPositionerProps()}>
		<ul
			{...menuApi.getContentProps()}
			class="bg-base-light border-base-border shadow-xs flex flex-col rounded-xl border p-1"
		>
			{#each [{ href: '/sign-in', label: 'Sign in' }, { href: '/sign-up', label: 'Sign up' }] as item (item.href)}
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
