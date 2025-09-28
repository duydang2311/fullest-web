<script lang="ts">
	import {
		ChartColumnOutline,
		EyeOutline,
		MessageSquareOutline,
		ShieldOutline,
		StarOutline,
		UsersOutline,
		ZapOutline,
	} from '~/lib/components/icons';

	const features = [
		{
			icon: EyeOutline,
			title: 'Public by Default',
			description:
				'Build trust with stakeholders through transparent project visibility and real-time progress tracking.',
		},
		{
			icon: StarOutline,
			title: 'Contributions',
			description:
				'Showcase your impact with a detailed contribution history that highlights your achievements and growth over time.',
		},
		{
			icon: UsersOutline,
			title: 'Collaboration',
			description:
				'Seamless collaboration tools that keep everyone aligned and productive, no matter where they work.',
		},
		{
			icon: ZapOutline,
			title: 'Fast',
			description:
				'Optimized performance ensures your team can focus on what matters most - getting things done.',
		},
		{
			icon: ShieldOutline,
			title: 'Security',
			description:
				'Bank-level security with granular permissions to keep your sensitive data protected.',
		},
		{
			icon: ChartColumnOutline,
			title: 'Analytics',
			description:
				'Gain insights into team performance and project health with comprehensive reporting tools.',
		},
		{
			icon: MessageSquareOutline,
			title: 'Notifications',
			description:
				'Stay informed without being overwhelmed. Intelligent notifications that adapt to your workflow.',
		},
	];
</script>

<section id="features" class="bg-base relative min-h-screen p-8 text-base-fg" data-theme="accent">
	<svg
		xmlns="http://www.w3.org/2000/svg"
		viewBox="0 0 1440 320"
		class="absolute inset-x-0 top-1 -translate-y-full text-base"
	>
		<path
			fill="currentColor"
			d="M0,192L34.3,192C68.6,192,137,192,206,192C274.3,192,343,192,411,213.3C480,235,549,277,617,272C685.7,267,754,213,823,202.7C891.4,192,960,224,1029,218.7C1097.1,213,1166,171,1234,154.7C1302.9,139,1371,149,1406,154.7L1440,160L1440,320L1405.7,320C1371.4,320,1303,320,1234,320C1165.7,320,1097,320,1029,320C960,320,891,320,823,320C754.3,320,686,320,617,320C548.6,320,480,320,411,320C342.9,320,274,320,206,320C137.1,320,69,320,34,320L0,320Z"
		></path>
	</svg>
	<div class="text-center">
		<h2 class="text-base-fg-strong font-bold leading-none tracking-tighter">
			Everything you need to manage tasks effectively
		</h2>
		<p class="mt-4">
			Powerful features designed to streamline your workflow and boost productivity.
		</p>
	</div>
	<div class="container mx-auto mt-8 grid grid-cols-[repeat(auto-fit,minmax(24rem,1fr))] gap-4">
		{#each features as feature (feature.title)}
			<div
				class="border-base-border hover:border-base-border-strong rounded-2xl border px-4 py-6 transition hover:shadow-sm"
			>
				<div class="bg-primary/10 flex h-12 w-12 items-center justify-center rounded-lg">
					<feature.icon class="text-primary h-6 w-6" />
				</div>
				<h3 class="text-base-fg-strong mt-2 tracking-tight">{feature.title}</h3>
				<p class="mt-6 text-pretty">{feature.description}</p>
			</div>
		{/each}
	</div>
</section>
