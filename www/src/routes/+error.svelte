<script lang="ts">
	import { page } from '$app/state';

	const snippets = {
		InvalidOAuthStateError: invalidOAuthState,
	};
	const snippet = $derived(
		page.error!.kind in snippets
			? snippets[page.error!.kind as keyof typeof snippets]
			: defaultSnippet
	);
</script>

{#snippet invalidOAuthState()}
	<h1>Invalid OAuth state parameter</h1>
{/snippet}

{#snippet defaultSnippet()}
	<h1>Oops! Something went wrong</h1>
	<pre>{JSON.stringify(page.error)}</pre>
{/snippet}

{@render snippet()}
