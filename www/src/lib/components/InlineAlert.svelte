<script lang="ts">
	import type { Component, Snippet } from 'svelte';
	import { ExclamationCircleOutline } from './icons';
	import type { SVGAttributes } from 'svelte/elements';

	interface Props {
		variant?: 'base' | 'negative';
		header: string;
		children: Snippet;
	}

	const { variant = 'base', header, children }: Props = $props();
	const icons: { [k in 'negative']: Component<SVGAttributes<SVGElement>> } = {
		negative: ExclamationCircleOutline,
	};
	const Icon = $derived(icons[variant as keyof typeof icons]);
</script>

<div class="border-base-border mt-4 rounded-lg border p-4">
	<div class="flex gap-4 max-sm:flex-row-reverse max-sm:justify-between">
		{#if Icon}
			<Icon class="c-inline-alert--icon c-inline-alert--icon--{variant}" />
		{/if}
		<div>
			<h3 class="c-inline-alert--header c-inline-alert--header--{variant}">{header}</h3>
			<div class="mt-2">
				{@render children()}
			</div>
		</div>
	</div>
</div>
