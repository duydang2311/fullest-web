import { browser } from '$app/environment';
import { gsap as __gsap } from 'gsap';
import { createGSAPTransition } from '@duydang2311/svutils';

export const gsap = {
	...__gsap,
	registerPlugin: (...args: object[]) => {
		if (browser) {
			__gsap.registerPlugin(...args);
		}
	},
};

export const tsap = createGSAPTransition(gsap);
