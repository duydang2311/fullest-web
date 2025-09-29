<script lang="ts">
	import { GitHub, GlobeOutline } from '~/lib/components/icons';
	import LogoType from '~/lib/components/LogoType.svelte';
	import { button } from '~/lib/utils/styles';
</script>

<footer class="border-t-base-border max-w-container-xl relative mx-auto border-t p-8">
	<LogoType />
	<p class="mt-1">Public-first task management built for transparency and collaboration.</p>
	<div class="mt-4 flex items-center gap-2">
		<a
			href="https://fullest-web.onrender.com"
			target="_blank"
			rel="noopener noreferrer"
			class="{button({ filled: true, outlined: true, icon: true })} inline-block"
		>
			<GlobeOutline />
		</a>
		<a
			href="https://github.com/duydang2311/fullest-web"
			target="_blank"
			rel="noopener noreferrer"
			class="{button({ filled: true, outlined: true, icon: true })} inline-block"
		>
			<GitHub />
		</a>
	</div>
	<p class="text-base-fg-muted mt-12">© 2025 Dang Quoc Duy, Inc. All rights reserved.</p>
</footer>
