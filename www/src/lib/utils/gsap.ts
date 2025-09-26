import { browser } from '$app/environment';
import { gsap as __gsap } from 'gsap';

export const gsap = {
	...__gsap,
	registerPlugin: (...args: object[]) => {
		if (browser) {
			__gsap.registerPlugin(...args);
		}
	},
};
