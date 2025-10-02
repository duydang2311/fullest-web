<script lang="ts">
	import { button } from '../utils/styles';
	import CreateActions from './AuthenticatedHeaderCreateActions.svelte';
	import AuthenticatedHeaderPfp from './AuthenticatedHeaderPfp.svelte';
	import LogoType from './LogoType.svelte';
</script>

<header class="border-b-base-border bg-base dark:bg-base-dark border-b px-8 py-4">
	<nav class="max-w-container-xl mx-auto flex w-full items-center justify-between gap-8">
		<div class="flex items-center gap-2">
			<LogoType />
			<a href="/" class="{button({ variant: 'base', ghost: true })} font-bold">Dashboard</a>
		</div>
		<div class="flex items-center gap-2">
			<CreateActions />
			<AuthenticatedHeaderPfp />
		</div>
	</nav>
</header>
