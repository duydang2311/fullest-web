<script lang="ts">
	import { EyeOutline, StarOutline } from '~/lib/components/icons';
	import { button } from '~/lib/utils/styles';
</script>

<div class="flex gap-4">
	<button
		type="button"
		class="{button({
			variant: 'base',
			filled: true,
			outlined: true,
		})} flex items-center gap-2 px-2 py-1.5 text-sm tracking-tight"
	>
		<EyeOutline />
		<span>Watch</span>
		<span
			class="text-base-fg bg-base-dark ml-2 inline-block h-full min-w-8 content-center rounded-sm px-2"
		>
			0
		</span>
	</button>
	<button
		type="button"
		class="{button({
			variant: 'base',
			filled: true,
			outlined: true,
		})} flex items-center gap-2 px-2 py-1.5 text-sm tracking-tight"
	>
		<StarOutline />
		<span>Star</span>
		<span
			class="text-base-fg bg-base-dark ml-2 inline-block h-full min-w-8 content-center rounded-sm px-2"
		>
			0
		</span>
	</button>
</div>
