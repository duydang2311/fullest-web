import { enhance } from '$app/forms';
import { type Attempt } from '@duydang2311/attempt';
import type { SubmitFunction } from '@sveltejs/kit';
import { type Attachment } from 'svelte/attachments';
import type { Validator } from './validation';
import { watch } from '@duydang2311/svutils';
import type { ValidationError } from './errors';

export type AppForm<T> = Attachment<HTMLFormElement> & AppFormProps<T>;
export type AppFormProps<T> = {
    touched: boolean;
    dirty: boolean;
    submitted: boolean;
    withEnhanced<
        Success extends Record<string, unknown> | undefined,
        Failure extends Record<string, unknown> | undefined,
    >(
        submitFn?: SubmitFunction<Success, Failure>
    ): Attachment<HTMLFormElement>;
    validate(value: unknown): Attempt<T, ValidationError>;
    syncErrors(f: () => Record<string, string[]> | undefined): AppForm<T>;
} & (
    | {
          data: T;
          errors?: never;
      }
    | {
          data?: never;
          errors: Record<string, string[]>;
      }
);

export interface CreateAppFormOptions<T> {
    validator: Validator<unknown, T>;
}

export function createForm<T>({ validator }: CreateAppFormOptions<T>): AppForm<T> {
    let touched = $state.raw(false);
    let dirty = $state.raw(false);
    let submitted = $state.raw(false);
    let values = $state.raw<{
        data?: T;
        errors?: Record<string, string[]>;
    }>();

    function validate(value: unknown) {
        const parsed = validator?.parse(value);
        if (parsed.ok) {
            values = { data: parsed.data };
        } else {
            values = { errors: parsed.error.errors };
        }
        return parsed;
    }

    function formValues(formEl: HTMLFormElement) {
        return Object.fromEntries(new FormData(formEl).entries());
    }

    function handleFocusIn() {
        touched = true;
    }

    function handleReset() {
        touched = false;
        dirty = false;
        submitted = false;
        values = undefined;
    }

    const attachment: Attachment<HTMLFormElement> = (node) => {
        function handleInput() {
            dirty = true;
            if (submitted) {
                validate(formValues(node));
            }
        }

        function handleSubmit() {
            submitted = true;
            validate(formValues(node));
        }

        node.addEventListener('focusin', handleFocusIn);
        node.addEventListener('input', handleInput);
        node.addEventListener('submit', handleSubmit);
        node.addEventListener('reset', handleReset);

        return () => {
            node.removeEventListener('focusin', handleFocusIn);
            node.removeEventListener('input', handleInput);
            node.removeEventListener('submit', handleSubmit);
            node.removeEventListener('reset', handleReset);
        };
    };

    const enhancedAttachment: Attachment<HTMLFormElement> = (node) => {
        function handleInput() {
            dirty = true;
            if (submitted) {
                validate(formValues(node));
            }
        }

        function handleSubmit() {
            submitted = true;
        }

        node.addEventListener('focusin', handleFocusIn);
        node.addEventListener('input', handleInput);
        node.addEventListener('submit', handleSubmit);
        node.addEventListener('reset', handleReset);

        return () => {
            node.removeEventListener('focusin', handleFocusIn);
            node.removeEventListener('input', handleInput);
            node.removeEventListener('submit', handleSubmit);
            node.removeEventListener('reset', handleReset);
        };
    };

    const appForm = Object.defineProperties(
        attachment,
        Object.getOwnPropertyDescriptors({
            get touched() {
                return touched;
            },
            get dirty() {
                return dirty;
            },
            get submitted() {
                return submitted;
            },
            set touched(value: boolean) {
                touched = value;
            },
            set dirty(value: boolean) {
                dirty = value;
            },
            set submitted(value: boolean) {
                submitted = value;
            },
            get data() {
                return values?.data;
            },
            set data(value: T | undefined) {
                values = value ? { data: value } : undefined;
            },
            get errors() {
                return values?.errors;
            },
            set errors(value: Record<string, string[]> | undefined) {
                values = value ? { errors: value } : undefined;
            },
            withEnhanced<
                Success extends Record<string, unknown> | undefined,
                Failure extends Record<string, unknown> | undefined,
            >(submitFn?: SubmitFunction<Success, Failure>) {
                return (node: HTMLFormElement) => {
                    const attached = enhancedAttachment(node);
                    const enhanced = enhance(node, (e) => {
                        if (values != null && values.errors) {
                            e.cancel();
                            return;
                        }
                        const validated = validate(formValues(node));
                        if (!validated.ok) {
                            e.cancel();
                            return;
                        }
                        return submitFn?.(e);
                    });
                    return () => {
                        attached?.();
                        enhanced.destroy();
                    };
                };
            },
            validate,
            syncErrors(f: () => Record<string, string[]> | undefined) {
                const errors = f();
                values = { errors };
                if (errors) {
                    touched = true;
                    dirty = true;
                    submitted = true;
                }
                watch.pre(f)(() => {
                    values = { errors: f() };
                });
                return appForm;
            },
        })
    ) as AppForm<T>;
    return appForm;
}
