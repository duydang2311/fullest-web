<script lang="ts">
	import LogoType from '~/lib/components/LogoType.svelte';
	import Footer from './Footer.svelte';
	import { button } from '~/lib/utils/styles';
	import Pfp from './Pfp.svelte';
	import CreateActions from './CreateActions.svelte';
</script>

<div class="flex min-h-screen flex-col">
	<header class="border-b-base-border border-b px-8 py-4">
		<nav class="max-w-container-xl mx-auto flex w-full items-center justify-between gap-8">
			<div class="flex items-center gap-2">
				<LogoType />
				<a href="/" class="{button({ variant: 'base', ghost: true })} font-bold">Dashboard</a>
			</div>
			<div class="flex items-center gap-2">
				<CreateActions />
				<Pfp />
			</div>
		</nav>
	</header>
	<main class="flex-1"></main>
	<Footer />
</div>
