<script lang="ts">
	import { page } from '$app/state';
	import { Tabs } from '~/lib/components/builders.svelte';
	import { button } from '~/lib/utils/styles';

	const { tabs }: { tabs: Tabs } = $props();
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
		class="border-b-base-border flex items-center border-b px-8 *:basis-32 max-md:*:flex-1 pb-1"
	>
		{#each tabItems as item}
			<button
				{...tabs.getTriggerProps({ value: item.href })}
				class="focus-visible:ring-focus-base-fg group relative focus-visible:outline-none"
			>
				{#if tabs.value === item.href}
					<div
						id="active-tab-underline"
						data-flip-id="active-tab-underline"
						class="bg-primary absolute inset-x-0 -bottom-1 h-[2px] rounded-full"
					></div>
				{/if}
				<a
					href={item.href}
					class="{button({
						variant: 'base',
						ghost: true,
					})} group-[[data-selected]]:text-base-fg-strong block group-[[data-selected]]:font-bold group-[[data-selected]]:tracking-tight"
				>
					{item.label}
				</a>
			</button>
		{/each}
	</div>
</div>
