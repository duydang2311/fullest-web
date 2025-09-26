<script lang="ts">
	import { goto, onNavigate } from '$app/navigation';
	import { page } from '$app/state';
	import { Flip } from 'gsap/dist/Flip';
	import Bg from '~/lib/assets/images/login_bg.jpeg';
	import { createTabs } from '~/lib/components/builders.svelte';
	import { gsap } from '~/lib/utils/gsap';
	import type { LayoutProps } from './$types';
	import { button } from '~/lib/utils/styles';

	const { children }: LayoutProps = $props();
	const id = $props.id();
	const tabs = createTabs({
		get id() {
			return id;
		},
		get value() {
			return page.url.pathname;
		},
		onValueChange: async (details) => {
			await goto(details.value, {
				keepFocus: true,
			});
		},
	});

	gsap.registerPlugin(Flip);

	onNavigate((navigation) => {
		if (navigation.willUnload) {
			return;
		}

		const state = Flip.getState('.active-tab');
		navigation.complete.then(() => {
			Flip.from(state, {
				targets: '.active-tab',
				duration: 0.25,
				ease: 'power2.inOut',
				scale: true,
			});
		});
	});
</script>

<main class="@container h-screen">
	<div>
		<img src={Bg} alt="Gradient background" class="fixed inset-0 min-h-full object-cover" />
	</div>
	<div class="relative h-full">
		<div class="bg-base max-w-160 @min-[40rem]:rounded-r-2xl flex h-full flex-col p-8">
			<div>
				<p class="text-base-fg-muted font-bold">FULLEST</p>
			</div>
			<div class="mt-8">
				{#if tabs.value === '/sign-in'}
					<h1 class="font-bold capitalize">Sign in</h1>
					<p class="text-base-fg-dim">Fill in your credentials or sign in with other accounts.</p>
				{:else if tabs.value === '/sign-up'}
					<h1 class="font-bold capitalize">Sign up</h1>
					<p class="text-base-fg-dim">Create a new account or sign up with other accounts.</p>
				{/if}
			</div>
			<div {...tabs.getRootProps()} class="mt-8 flex flex-1 flex-col">
				<div
					{...tabs.getListProps()}
					class="bg-base-light border-base-border flex gap-1 rounded-2xl border p-1"
				>
					{#each [{ value: '/sign-in', label: 'Sign in' }, { value: '/sign-up', label: 'Sign up' }] as tab (tab.value)}
						<button
							{...tabs.getTriggerProps({ value: tab.value })}
							class="{button({
								variant: 'base',
							})} hover:bg-base-hover active:bg-base-active group relative flex-1 focus-visible:ring-0 focus-visible:ring-offset-0"
						>
							{#if tabs.value === tab.value}
								<div
									class="active-tab bg-base-selected group-focus-visible:ring-focus-base-fg-muted absolute inset-0 rounded-xl transition-[box-shadow]"
									data-flip-id="active-tab"
								></div>
							{/if}
							<span class="relative">
								{tab.label}
							</span>
						</button>
					{/each}
				</div>
				<div {...tabs.getContentProps({ value: tabs.value! })} class="mt-4 flex-1">
					{@render children()}
				</div>
			</div>
		</div>
	</div>
</main>
