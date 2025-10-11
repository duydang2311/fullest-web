<script lang="ts">
	import { CheckCircleOutline } from '~/lib/components/icons';
	import ProjectActions from './ProjectActions.svelte';
	import Tabs from './Tabs.svelte';
	import { goto, onNavigate } from '$app/navigation';
	import { gsap } from '~/lib/utils/gsap';
	import { Flip } from 'gsap/dist/Flip';
	import LogoType from '~/lib/components/LogoType.svelte';
	import AuthenticatedHeaderCreateActions from '~/lib/components/AuthenticatedHeaderCreateActions.svelte';
	import AuthenticatedHeaderPfp from '~/lib/components/AuthenticatedHeaderPfp.svelte';
	import Footer from '~/lib/components/Footer.svelte';
	import { createTabs } from '~/lib/components/builders.svelte';
	import { page } from '$app/state';
	import { indexOf } from '~/lib/utils/string';

	const { data, children } = $props();
	const id = $props.id();
	const tabs = createTabs({
		id,
		get value() {
			const idx = indexOf(page.url.pathname, '/'.charCodeAt(0), 3);
			if (idx === -1) {
				return page.url.pathname;
			}
			return page.url.pathname.substring(0, idx);
		},
		onValueChange: async (details) => {
			await goto(details.value, { keepFocus: true });
		},
	});

	gsap.registerPlugin(Flip);

	onNavigate((e) => {
		if (e.willUnload) {
			return;
		}

		const state = Flip.getState('#active-tab-underline');
		e.complete.then(() => {
			Flip.from(state, {
				targets: '#active-tab-underline',
				duration: 0.3,
				ease: 'circ.inOut',
			});
		});
	});
</script>

<div class="flex min-h-screen flex-col">
	<header>
		<nav class="pt-4">
			<div class="flex items-center justify-between gap-8 px-8">
				<div class="flex items-center gap-4">
					<LogoType />
					<div class="flex items-center gap-1">
						<a href="/{data.namespace.user.name}" class="c-link">
							{data.namespace.kind === 'user' ? data.namespace.user.name : ''}
						</a>
						<span class="text-base-fg-muted">/</span>
						<a
							href="/{data.namespace.user.name}/{data.project.identifier}"
							class="c-link not-hover:text-base-fg-strong font-bold"
						>
							{data.project.name}
						</a>
					</div>
				</div>
				<div class="flex items-center gap-4">
					<AuthenticatedHeaderCreateActions />
					<AuthenticatedHeaderPfp />
				</div>
			</div>
			<div class="mt-2">
				<Tabs {tabs} />
			</div>
		</nav>
	</header>
	<main class="bg-base-light flex flex-1 flex-col px-8 py-4">
		<div
			{...tabs.getContentProps({ value: tabs.value! })}
			class="max-w-container-xl mx-auto flex w-full flex-1 flex-col"
		>
			{@render children()}
		</div>
	</main>
	<Footer />
</div>
