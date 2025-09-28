<script lang="ts">
	import type { HTMLAttributes } from 'svelte/elements';
	import { Logo } from '~/lib/components/icons';

	const { class: cls, ...props }: HTMLAttributes<HTMLDivElement> = $props();
</script>

<div {...props} class="flex items-center gap-1 {cls}">
	<Logo />
	<p class="text-base-fg-strong font-black tracking-tight">FULLEST</p>
</div>
