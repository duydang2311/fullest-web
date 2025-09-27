<script lang="ts">
	import Bg from '~/lib/assets/images/login_bg.jpeg';
	import type { LayoutProps } from './$types';

	const { children }: LayoutProps = $props();
</script>

<main class="h-screen">
	<div>
		<img
			src={Bg}
			alt="Gradient background"
			class="fixed inset-0 min-h-full object-cover will-change-transform"
		/>
	</div>
	<div class="relative h-full">
		<div
			class="bg-base max-w-160 min-[40rem]:rounded-r-3xl relative flex h-full min-h-max flex-col overflow-hidden p-8 pt-20"
		>
			<svg
				xmlns="http://www.w3.org/2000/svg"
				viewBox="0 0 1440 320"
				class="text-primary absolute inset-x-0 top-0 h-auto"
			>
				<path
					fill="currentColor"
					d="M0,32L20,37.3C40,43,80,53,120,74.7C160,96,200,128,240,122.7C280,117,320,75,360,53.3C400,32,440,32,480,74.7C520,117,560,203,600,208C640,213,680,139,720,122.7C760,107,800,149,840,186.7C880,224,920,256,960,240C1000,224,1040,160,1080,165.3C1120,171,1160,245,1200,282.7C1240,320,1280,320,1320,282.7C1360,245,1400,171,1420,133.3L1440,96L1440,0L1420,0C1400,0,1360,0,1320,0C1280,0,1240,0,1200,0C1160,0,1120,0,1080,0C1040,0,1000,0,960,0C920,0,880,0,840,0C800,0,760,0,720,0C680,0,640,0,600,0C560,0,520,0,480,0C440,0,400,0,360,0C320,0,280,0,240,0C200,0,160,0,120,0C80,0,40,0,20,0L0,0Z"
				></path>
			</svg>
			<!-- <svg
				xmlns="http://www.w3.org/2000/svg"
				viewBox="0 0 1440 320"
				class="text-primary absolute inset-x-0 bottom-0 h-auto"
			>
				<path
					fill="currentColor"
					d="M0,224L34.3,240C68.6,256,137,288,206,282.7C274.3,277,343,235,411,218.7C480,203,549,213,617,224C685.7,235,754,245,823,229.3C891.4,213,960,171,1029,170.7C1097.1,171,1166,213,1234,229.3C1302.9,245,1371,235,1406,229.3L1440,224L1440,320L1405.7,320C1371.4,320,1303,320,1234,320C1165.7,320,1097,320,1029,320C960,320,891,320,823,320C754.3,320,686,320,617,320C548.6,320,480,320,411,320C342.9,320,274,320,206,320C137.1,320,69,320,34,320L0,320Z"
				></path>
			</svg> -->
			<div class="relative flex flex-1 flex-col">
				<p class="text-base-fg-muted mb-8 font-bold">FULLEST</p>
				{@render children()}
			</div>
		</div>
	</div>
</main>
