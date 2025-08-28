import * as v from 'valibot';
import { createValidator } from './validation';

export type ProblemDetails = v.InferOutput<typeof problemDetailsSchema>;

const problemDetailsSchema = v.object({
	status: v.number(),
	title: v.string(),
	detail: v.optional(v.string()),
	type: v.optional(v.string()),
	instance: v.optional(v.string()),
	traceId: v.optional(v.string()),
	errors: v.optional(
		v.array(
			v.object({
				field: v.string(),
				code: v.string(),
			})
		)
	),
});

export const problemDetailsValidator = createValidator(problemDetailsSchema);
