import { browser } from '$app/environment';
import { createGSAPTransition } from '@duydang2311/svutils';
import { gsap as __gsap } from 'gsap';
export { Flip } from 'gsap/dist/Flip';

export const gsap = {
    ...__gsap,
    registerPlugin: (...args: object[]) => {
        if (browser) {
            __gsap.registerPlugin(...args);
        }
    },
};

export const tsap = createGSAPTransition(gsap);
