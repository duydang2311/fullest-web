import { attempt, type Attempt } from '@duydang2311/attempt';
import * as v from 'valibot';
import { ValidationError } from './errors';

export interface Validator<TInput, TOutput> {
	check(value: unknown): value is TInput;
	parse(value: unknown): Attempt<TOutput, ValidationError>;
}

export interface AsyncValidator<T> {
	parseAsync(value: unknown): Promise<Attempt<T, ValidationError>>;
}

export const createValidator: {
	<TSchema extends v.BaseSchema<unknown, unknown, v.BaseIssue<unknown>>>(
		schema: TSchema
	): Validator<v.InferInput<TSchema>, v.InferOutput<TSchema>>;
	<TSchema extends v.BaseSchemaAsync<unknown, unknown, v.BaseIssue<unknown>>>(
		schema: TSchema
	): AsyncValidator<v.InferOutput<TSchema>>;
} = (
	schema:
		| v.BaseSchema<unknown, unknown, v.BaseIssue<unknown>>
		| v.BaseSchemaAsync<unknown, unknown, v.BaseIssue<unknown>>
) => {
	return (schema.async ? new AsyncValibotValidator(schema) : new ValibotValidator(schema)) as never;
};

class ValibotValidator<TInput, TOutput> implements Validator<TInput, TOutput> {
	readonly #schema: v.BaseSchema<TInput, TOutput, v.BaseIssue<unknown>>;

	constructor(schema: v.BaseSchema<TInput, TOutput, v.BaseIssue<unknown>>) {
		this.#schema = schema;
	}

	public check(value: unknown) {
		return v.is(this.#schema, value);
	}

	public parse(value: unknown) {
		const result = v.safeParse(this.#schema, value);
		if (result.success) {
			return attempt.ok(result.output);
		}
		return attempt.fail(transformValibotIssues(result.issues));
	}
}

class AsyncValibotValidator<T> implements AsyncValidator<T> {
	readonly #schema: v.BaseSchemaAsync<unknown, T, v.BaseIssue<unknown>>;

	constructor(schema: v.BaseSchemaAsync<unknown, T, v.BaseIssue<unknown>>) {
		this.#schema = schema;
	}

	public async parseAsync(value: unknown) {
		const result = await v.safeParseAsync(this.#schema, value);
		if (result.success) {
			return attempt.ok(result.output);
		}
		return attempt.fail(transformValibotIssues(result.issues));
	}
}

const transformValibotIssues = (issues: v.BaseIssue<unknown>[]) => {
	return ValidationError(
		issues.reduce<Record<string, string[]>>((acc, cur) => {
			const path = v.getDotPath(cur) ?? '$';
			if (!acc[path]) {
				acc[path] = [cur.type];
			} else {
				acc[path].push(cur.type);
			}
			return acc;
		}, {})
	);
};
