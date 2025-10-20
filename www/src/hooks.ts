import { attempt, type Attempt } from '@duydang2311/attempt';
import type { Transport } from '@sveltejs/kit';

export const transport: Transport = {
	Attempt: {
		encode: (att) => {
			if (attempt.check(att) && 'pipe' in att) {
				// eslint-disable-next-line @typescript-eslint/no-unused-vars
				const { pipe, ...rest } = att;
				return rest;
			}
		},
		decode: (att: Attempt) => {
			if (att.ok) {
				return attempt.ok(att.data);
			} else {
				return attempt.fail(att.error);
			}
		},
	},
};
