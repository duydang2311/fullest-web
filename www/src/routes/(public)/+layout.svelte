<script lang="ts">
	import Bg from '~/lib/assets/images/login_bg.jpeg';
	import type { LayoutProps } from './$types';

	const { children }: LayoutProps = $props();
</script>

<main class="h-screen">
	<div>
		<img src={Bg} alt="Gradient background" class="fixed inset-0 min-h-full object-cover" />
	</div>
	<div class="relative h-full">
		<div
			class="bg-base max-w-160 min-[40rem]:rounded-r-2xl relative flex h-full min-h-max flex-col overflow-hidden p-8"
		>
			<svg
				xmlns="http://www.w3.org/2000/svg"
				viewBox="0 0 1440 320"
				class="text-primary absolute inset-x-0 h-auto -translate-y-8"
			>
				<path
					fill="currentColor"
					fill-opacity="1"
					d="M0,32L15,48C30,64,60,96,90,101.3C120,107,150,85,180,69.3C210,53,240,43,270,37.3C300,32,330,32,360,48C390,64,420,96,450,106.7C480,117,510,107,540,138.7C570,171,600,245,630,256C660,267,690,213,720,192C750,171,780,181,810,202.7C840,224,870,256,900,272C930,288,960,288,990,277.3C1020,267,1050,245,1080,202.7C1110,160,1140,96,1170,106.7C1200,117,1230,203,1260,240C1290,277,1320,267,1350,224C1380,181,1410,107,1425,69.3L1440,32L1440,0L1425,0C1410,0,1380,0,1350,0C1320,0,1290,0,1260,0C1230,0,1200,0,1170,0C1140,0,1110,0,1080,0C1050,0,1020,0,990,0C960,0,930,0,900,0C870,0,840,0,810,0C780,0,750,0,720,0C690,0,660,0,630,0C600,0,570,0,540,0C510,0,480,0,450,0C420,0,390,0,360,0C330,0,300,0,270,0C240,0,210,0,180,0C150,0,120,0,90,0C60,0,30,0,15,0L0,0Z"
				></path>
			</svg>
			<svg
				xmlns="http://www.w3.org/2000/svg"
				viewBox="0 0 1440 320"
				class="text-primary absolute inset-x-0 bottom-0 h-auto translate-y-0"
			>
				<path
					fill="currentColor"
					d="M0,224L34.3,240C68.6,256,137,288,206,282.7C274.3,277,343,235,411,218.7C480,203,549,213,617,224C685.7,235,754,245,823,229.3C891.4,213,960,171,1029,170.7C1097.1,171,1166,213,1234,229.3C1302.9,245,1371,235,1406,229.3L1440,224L1440,320L1405.7,320C1371.4,320,1303,320,1234,320C1165.7,320,1097,320,1029,320C960,320,891,320,823,320C754.3,320,686,320,617,320C548.6,320,480,320,411,320C342.9,320,274,320,206,320C137.1,320,69,320,34,320L0,320Z"
				></path>
			</svg>
			<div class="relative flex flex-1 flex-col">
				{@render children()}
			</div>
		</div>
	</div>
</main>
