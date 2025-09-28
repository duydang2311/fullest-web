<script lang="ts">
	import { GitHubDark, GitHubLight, GlobeOutline } from '~/lib/components/icons';
	import LogoType from '~/lib/components/LogoType.svelte';
	import { button } from '~/lib/utils/styles';
</script>

<footer class="border-t-base-border relative border-t p-8">
	<LogoType />
	<p class="mt-1">Public-first task management built for transparency and collaboration.</p>
	<div class="mt-4 flex items-center gap-2">
		<a
			href="https://fullest-web.onrender.com"
			target="_blank"
			rel="noopener noreferrer"
			class="{button({ filled: true, outlined: true })} inline-block"
		>
			<GlobeOutline />
		</a>
		<a
			href="https://github.com/duydang2311/fullest-web"
			target="_blank"
			rel="noopener noreferrer"
			class="{button({ filled: true, outlined: true })} inline-block"
		>
			<GitHubLight class="dark:hidden" />
			<GitHubDark class="not-dark:hidden" />
		</a>
	</div>
	<p class="text-base-fg-muted mt-12">© 2025 Dang Quoc Duy, Inc. All rights reserved.</p>
</footer>
