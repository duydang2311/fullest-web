<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/state';
	import type { Snippet } from 'svelte';
	import { createTabs } from '~/lib/components/builders.svelte';
	import { button } from '~/lib/utils/styles';

	const { children }: { children: Snippet } = $props();
	const id = $props.id();
	const tabs = createTabs({
		id,
		get value() {
			return page.url.pathname;
		},
		onValueChange: async (details) => {
			await goto(details.value, { keepFocus: true });
		},
	});
	const tabItems = [
		{ label: 'Overview', href: `/${page.params.namespace}/${page.params.slug}` },
		{ label: 'Tasks', href: `/${page.params.namespace}/${page.params.slug}/tasks` },
		{ label: 'Activity', href: `/${page.params.namespace}/${page.params.slug}/activity` },
		{ label: 'Contributors', href: `/${page.params.namespace}/${page.params.slug}/contributors` },
	];
</script>

<div {...tabs.getRootProps()}>
	<div
		{...tabs.getListProps()}
		class="border-b-base-border flex items-center border-b *:basis-40 max-md:*:flex-1"
	>
		{#each tabItems as item}
			<button
				{...tabs.getTriggerProps({ value: item.href })}
				class="focus-visible:ring-focus-base-fg-muted group relative focus-visible:outline-none"
			>
				{#if tabs.value === item.href}
					<div
						id="active-tab-underline"
						data-flip-id="active-tab-underline"
						class="bg-base-fg-strong absolute inset-x-0 -bottom-[2px] h-[2px]"
					></div>
				{/if}
				<a
					href={item.href}
					class="{button({
						variant: 'base',
						ghost: true,
					})} group-[[data-selected]]:text-base-fg-strong group-[[data-selected]]:bg-base-dark block rounded-none group-[[data-selected]]:font-bold group-[[data-selected]]:tracking-tight"
				>
					{item.label}
				</a>
			</button>
		{/each}
	</div>
	<div {...tabs.getContentProps({ value: tabs.value! })} class="mt-4 flex-1">
		{@render children()}
	</div>
</div>
