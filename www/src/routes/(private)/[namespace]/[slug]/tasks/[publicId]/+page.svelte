<script lang="ts">
	import Stats from './Stats.svelte';

	const { data } = $props();
</script>

<div class="divide-base-border-weak -my-4 flex-1 *:py-4 md:flex md:divide-x">
	<div class="flex-1 md:pr-8">
		<div class="hidden items-center gap-4 md:flex">
			<img
				src="https://avatars.githubusercontent.com/u/34796192?v=4"
				class="size-avatar-md rounded-full"
			/>
			<div>
				<h1>
					<span class="text-title-sm text-base-fg-strong font-bold">{data.task.title}</span>
					<span class="c-help-text inline-block align-super">#{data.task.publicId}</span>
				</h1>
				<div class="c-help-text flex items-center gap-2">
					<span>Created 2 days ago by <strong class="text-base-fg-strong">somebody</strong></span>
					<span>•</span>
					<span>Last updated 3 hours ago</span>
				</div>
			</div>
		</div>
		<div class="md:hidden">
			<div class="flex gap-4">
				<img
					src="https://avatars.githubusercontent.com/u/34796192?v=4"
					class="size-avatar-sm rounded-full"
				/>
				<div>
					<h1>
						<span class="text-title-sm text-base-fg-strong font-bold"
							>{data.task.title} Lorem ipsum dolor sit amet, hello world!</span
						>
						<span class="c-help-text inline-block align-super">#{data.task.publicId}</span>
					</h1>
				</div>
			</div>
			<div class="mt-4 flex flex-wrap gap-x-8 gap-y-2 *:min-w-40 md:hidden">
				<Stats />
			</div>
			<div class="c-help-text mt-2 flex items-center gap-2 md:mt-1 md:hidden">
				<span>Created 2 days ago by <strong class="text-base-fg-strong">somebody</strong></span>
				<span>•</span>
				<span>Last updated 3 hours ago</span>
			</div>
		</div>
		<hr class="border-base-border-weak -mx-8 mt-4 md:hidden" />
		<article class="border-base-border mt-4 rounded-lg border p-4">
			Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut
			labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco
			laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in
			voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat
			non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem, ipsum
			dolor sit amet consectetur adipisicing elit. Maxime magnam debitis officia, iure quidem velit?
			Aliquid, ipsa accusantium. Iste soluta, nisi cum incidunt tempora quod sed adipisci odit
			fugiat omnis? Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor
			incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud
			exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
			reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint
			occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est
			laborum. Lorem, ipsum dolor sit amet consectetur adipisicing elit. Maxime magnam debitis
			officia, iure quidem velit? Aliquid, ipsa accusantium. Iste soluta, nisi cum incidunt tempora
			quod sed adipisci odit fugiat omnis? Lorem ipsum dolor sit amet, consectetur adipiscing elit,
			sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam,
			quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute
			irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
			Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim
			id est laborum. Lorem, ipsum dolor sit amet consectetur adipisicing elit. Maxime magnam
			debitis officia, iure quidem velit? Aliquid, ipsa accusantium. Iste soluta, nisi cum incidunt
			tempora quod sed adipisci odit fugiat omnis?
		</article>
	</div>
	<div class="min-w-64 md:pl-8">
		<div class="hidden flex-col gap-4 md:flex">
			<Stats />
		</div>
	</div>
</div>
