<script lang="ts">
	import Flip from 'gsap/dist/Flip';
	import { untrack } from 'svelte';
	import { gsap } from '~/lib/utils/gsap';

	gsap.registerPlugin(Flip);

	const texts = [
		'transparency',
		'productivity',
		'community',
		'efficiency',
		'reliability',
		'creativity',
	];
	let index = 0;
</script>

<div
	class="bg-primary/15 text-primary ml-0.5 px-2"
	{@attach (node) => {
		return untrack(() => {
			const interval = setInterval(() => {
				index = (index + 1) % texts.length;
				const child = node.firstChild!;
				const oldWidth = node.clientWidth;
				const oldContent = child.textContent;
				child.textContent = texts[index];

				const tl = gsap.timeline();
				const newWidth = node.clientWidth;
				child.textContent = oldContent;
				tl.set(node, { width: oldWidth, overflow: 'hidden' })
					.to(child, {
						duration: 0.2,
						yPercent: 100,
						ease: 'circ.inOut',
						onComplete: () => {
							child.textContent = texts[index];
						},
					})
					.set(child, {
						opacity: 0,
						yPercent: -40,
					})
					.to(node, {
						borderRadius: '1rem',
						scale: 0.96,
						duration: 0.4,
						ease: 'circ.inOut',
					})
					.to(
						node,
						{
							width: newWidth,
							ease: 'power3.inOut',
							clearProps: 'width',
							duration: 0.4,
						},
						'0.2'
					)
					.to(
						child,
						{
							opacity: 1,
							yPercent: 0,
							duration: 0.4,
							ease: 'circ.inOut',
						},
						'0.4'
					)
					.to(
						node,
						{ borderRadius: 0, duration: 0.2, rotate: 0, scale: 1, ease: 'circ.out' },
						'0.8'
					)
					.set(node, { clearProps: 'overflow,borderRadius,scale,rotate' });
			}, 3000);

			return () => clearInterval(interval);
		});
	}}
>
	<span class="inline-block leading-normal">
		{texts[index]}
	</span>
</div>
