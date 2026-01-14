import { v } from '~/lib/utils/valibot';
import { createValidator } from '~/lib/utils/validation';

export const validator = createValidator(
    v.object({
        name: v.pipe(v.string(), v.minLength(3), v.maxLength(100)),
        identifier: v.string(),
        summary: v.optional(v.pipe(v.string(), v.maxLength(350))),
    })
);
