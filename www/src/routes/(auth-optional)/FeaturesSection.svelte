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

<section
	id="features"
	class="bg-base text-base-fg relative min-h-screen p-8"
	data-theme="accent"
>
	<!-- https://www.fffuel.co/sssurf/ -->
	<svg
		xmlns="http://www.w3.org/2000/svg"
		version="1.1"
		xmlns:xlink="http://www.w3.org/1999/xlink"
		viewBox="0 0 2400 800"
		opacity="1"
		class="absolute inset-x-0 top-1 -z-10 -translate-y-3/4 text-base"
	>
		<defs>
			<linearGradient x1="50%" y1="0%" x2="50%" y2="100%" id="sssurf-grad">
				<stop stop-color="var(--color-base)" stop-opacity="1" offset="0%"></stop>
				<stop stop-color="var(--color-base)" stop-opacity="1" offset="100%"></stop>
			</linearGradient>
		</defs><g fill="url(#sssurf-grad)" transform="matrix(1,0,0,1,8.7225341796875,277.5447692871094)"
			><path
				d="M-10,10C42.08333333333333,18.333333333333336,137.91666666666666,62.29166666666667,240,50C342.08333333333337,37.70833333333333,380,-54.625,480,-49C580,-43.375,620,69.08333333333333,720,77C820,84.91666666666667,860,-7.041666666666667,960,-11C1060,-14.958333333333332,1100,57.375,1200,58C1300,58.625,1340,-11.125,1440,-8C1540,-4.875,1580,84.45833333333333,1680,73C1780,61.54166666666667,1820,-66.95833333333333,1920,-63C2020,-59.041666666666664,2060,79.08333333333333,2160,92C2260,104.91666666666667,2297.9166666666665,-65.16666666666667,2400,-1C2502.0833333333335,63.16666666666667,3254.1666666666665,212.29166666666669,2650,400C2045.8333333333335,587.7083333333333,156.25,795.8333333333334,-500,900"
				transform="matrix(1,0,0,1,0,52)"
				opacity="0.04"
			></path><path
				d="M-10,10C42.08333333333333,18.333333333333336,137.91666666666666,62.29166666666667,240,50C342.08333333333337,37.70833333333333,380,-54.625,480,-49C580,-43.375,620,69.08333333333333,720,77C820,84.91666666666667,860,-7.041666666666667,960,-11C1060,-14.958333333333332,1100,57.375,1200,58C1300,58.625,1340,-11.125,1440,-8C1540,-4.875,1580,84.45833333333333,1680,73C1780,61.54166666666667,1820,-66.95833333333333,1920,-63C2020,-59.041666666666664,2060,79.08333333333333,2160,92C2260,104.91666666666667,2297.9166666666665,-65.16666666666667,2400,-1C2502.0833333333335,63.16666666666667,3254.1666666666665,212.29166666666669,2650,400C2045.8333333333335,587.7083333333333,156.25,795.8333333333334,-500,900"
				transform="matrix(1,0,0,1,0,104)"
				opacity="0.6"
			></path><path
				d="M-10,10C42.08333333333333,18.333333333333336,137.91666666666666,62.29166666666667,240,50C342.08333333333337,37.70833333333333,380,-54.625,480,-49C580,-43.375,620,69.08333333333333,720,77C820,84.91666666666667,860,-7.041666666666667,960,-11C1060,-14.958333333333332,1100,57.375,1200,58C1300,58.625,1340,-11.125,1440,-8C1540,-4.875,1580,84.45833333333333,1680,73C1780,61.54166666666667,1820,-66.95833333333333,1920,-63C2020,-59.041666666666664,2060,79.08333333333333,2160,92C2260,104.91666666666667,2297.9166666666665,-65.16666666666667,2400,-1C2502.0833333333335,63.16666666666667,3254.1666666666665,212.29166666666669,2650,400C2045.8333333333335,587.7083333333333,156.25,795.8333333333334,-500,900"
				transform="matrix(1,0,0,1,0,156)"
				opacity="1.00"
			></path></g
		></svg
	>
	<div class="text-center">
		<h2 class="text-base-fg-strong font-bold leading-none tracking-tighter">
			Everything you need to manage tasks effectively
		</h2>
		<p class="mt-4">
			Powerful features designed to streamline your workflow and boost productivity.
		</p>
	</div>
	<div class="max-w-container-xl mx-auto mt-8 grid md:grid-cols-2 lg:grid-cols-3 gap-4">
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
