<script lang="ts">
	import { enhance } from '$app/forms';
	import { page } from '$app/state';
	import slug from 'slug';
	import AuthenticatedHeader from '~/lib/components/AuthenticatedHeader.svelte';
	import Footer from '~/lib/components/Footer.svelte';
	import { PlusOutline } from '~/lib/components/icons';
	import { button, field, input } from '~/lib/utils/styles';
	import type { PageProps } from './$types';

	const { data, form }: PageProps = $props();
	let description = $state.raw<string>();
	let name = $state.raw<string>();
	let identifier = $derived(name ? slug(name) : null);
	const fieldErrors = $derived(form?.errors ?? {});
</script>

<div class="flex min-h-screen flex-col">
	<AuthenticatedHeader />
	<main class="bg-base-light dark:bg-base flex-1 px-8">
		<form method="post" class="max-w-container-xl mx-auto flex-1 py-8" use:enhance>
			<div class="max-w-container-sm">
				<div>
					<h1 class="text-title-sm text-base-fg-strong font-bold">Create a new project</h1>
					<p class="text-base-fg-muted text-body-sm mt-1 italic">
						Required fields are marked with an asterisk (*).
					</p>
				</div>
				<fieldset class="mt-8">
					<legend
						class="text-title-xs text-base-fg-strong flex w-full items-center gap-2 font-semibold"
					>
						General
					</legend>
					<div class="mt-4 space-y-4">
						<div class={field()}>
							<label for="name" class="c-label">Project name *</label>
							<input id="name" name="name" type="text" class={input()} bind:value={name} required />
							{#if identifier}
								<p class="c-help-text">
									Project will be created at: <strong
										>{page.url.origin}/{data.username}/{identifier}</strong
									>.
								</p>
								<input type="hidden" name="identifier" value={identifier} />
							{/if}
							{#if fieldErrors.name}
								<p class="c-help-text text-negative">{fieldErrors.name.join(', ')}.</p>
							{/if}
						</div>
						<div class={field()}>
							<label for="description" class="c-label">Description</label>
							<input
								id="description"
								name="description"
								type="text"
								class={input()}
								bind:value={description}
								maxlength="350"
							/>
							{#if fieldErrors.description}
								<p class="c-help-text text-negative">{fieldErrors.description.join(', ')}.</p>
							{/if}
							<p class="c-help-text">
								<strong>{description?.length ?? 0}</strong> / 350 characters
							</p>
						</div>
						<button
							type="submit"
							class="{button({
								variant: 'primary',
								filled: true,
								outlined: true,
							})} ml-auto flex items-center gap-2"
						>
							<PlusOutline />
							<span>Create project</span>
						</button>
					</div>
				</fieldset>
				{#if form}
					<p class="c-help-text text-negative mt-4">
						An error occurred while creating the project: {form.kind}.
					</p>
				{/if}
			</div>
		</form>
	</main>
	<Footer />
</div>
