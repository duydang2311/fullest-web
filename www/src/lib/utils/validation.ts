import { attempt, type Attempt } from '@duydang2311/attempt';
import type { StandardSchemaV1 } from '@standard-schema/spec';
import { ValidationError } from './errors';

export interface Validator<T> {
    check(value: unknown): value is T;
    parse(value: unknown): Attempt<T, ValidationError>;
}

export type InferOutput<T extends Validator<unknown>> =
    T extends Validator<infer TOutput> ? TOutput : never;

export interface AsyncValidator<T> {
    parseAsync(value: unknown): Promise<Attempt<T, ValidationError>>;
}

export function createValidator<T extends StandardSchemaV1>(schema: T) {
    return new StandardValidator(schema) as Validator<
        T extends StandardSchemaV1<unknown, infer O> ? O : never
    >;
}

class StandardValidator<TOutput> implements Validator<TOutput> {
    readonly #schema: StandardSchemaV1<unknown, TOutput>;

    constructor(schema: StandardSchemaV1<unknown, TOutput>) {
        this.#schema = schema;
    }

    public check(value: unknown): value is TOutput {
        const result = this.#schema['~standard'].validate(value);
        if (result instanceof Promise) {
            throw new TypeError('Schema validation must be synchronous');
        }
        return result.issues ? false : true;
    }

    public parse(value: unknown) {
        const result = this.#schema['~standard'].validate(value);
        if (result instanceof Promise) {
            throw new TypeError('Schema validation must be synchronous');
        }
        return result.issues
            ? attempt.fail(
                  ValidationError(
                      result.issues.reduce(
                          (acc, cur) => {
                              const k = cur.path?.join('.') ?? '$';
                              if (!acc[k]) {
                                  acc[k] = [cur.message];
                              } else {
                                  acc[k].push(cur.message);
                              }
                              return acc;
                          },
                          {} as Record<string, string[]>
                      )
                  )
              )
            : attempt.ok(result.value);
    }
}
