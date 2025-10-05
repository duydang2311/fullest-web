import isNetworkError from 'is-network-error';
import * as v from 'valibot';
import type { ProblemDetails } from './problem';
import { createValidator } from './validation';

export const ErrorKind = {
	AbortError: 'AbortError',
	HttpNetworkError: 'HttpNetworkError',
	HttpUnknownError: 'HttpUnknownError',
	JsonSyntaxError: 'JsonSyntaxError',
	JsonUnknownError: 'JsonUnknownError',
	InternalServerError: 'InternalServerError',
	MissingOAuthStateError: 'MissingOAuthStateError',
	MismatchOAuthStateError: 'MismatchOAuthStateError',
	MissingGoogleAuthorizationCodeError: 'MissingGoogleAuthorizationCodeError',
	ValidationError: 'ValidationError',
	GoogleOAuthExchangeError: 'GoogleOAuthExchangeError',
	GenericError: 'GenericError',
	NotFoundError: 'NotFoundError',
} as const;

export const ErrorCode = {
	Invalid: 'ERR_INVALID',
	Required: 'ERR_REQUIRED',
	MinLength: 'ERR_MIN_LENGTH',
	MaxLength: 'ERR_MAX_LENGTH',
	Conflict: 'ERR_CONFLICT',
	Json: 'ERR_JSON',
	NotFound: 'ERR_NOT_FOUND',
	UserNotFound: 'ERR_USER_NOT_FOUND',
} as const;

export type ErrorCode = typeof ErrorCode;
export type ErrorKind = typeof ErrorKind;

export interface AbortError extends App.Error {
	kind: ErrorKind['AbortError'];
}

export interface HttpNetworkError extends App.Error {
	kind: ErrorKind['HttpNetworkError'];
}

export interface HttpUnknownError extends App.Error {
	kind: ErrorKind['HttpUnknownError'];
	details?: unknown;
}

export interface JsonSyntaxError extends App.Error {
	kind: ErrorKind['JsonSyntaxError'];
}

export interface JsonUnknownError extends App.Error {
	kind: ErrorKind['JsonUnknownError'];
	details?: unknown;
}

export interface InternalServerError extends App.Error {
	kind: ErrorKind['InternalServerError'];
}

export interface MissingOAuthStateError extends App.Error {
	kind: ErrorKind['MissingOAuthStateError'];
}

export interface MismatchOAuthStateError extends App.Error {
	kind: ErrorKind['MismatchOAuthStateError'];
}

export interface MissingGoogleAuthorizationCodeError extends App.Error {
	kind: ErrorKind['MissingGoogleAuthorizationCodeError'];
}

export interface ValidationError extends App.Error {
	kind: ErrorKind['ValidationError'];
	errors: Record<string, string[]>;
	detail?: string;
}

export interface GenericError extends App.Error {
	kind: ErrorKind['GenericError'];
	context: unknown;
}

export interface NotFoundError extends App.Error {
	kind: ErrorKind['NotFoundError'];
}

const error = <T>(object: T) => ({
	message: '', // bypassing App.Error required property
	...object,
});

type RichError<T> = T & { context: { step: string } };

export const AbortError = (): AbortError => error({ kind: ErrorKind.AbortError });
export const HttpNetworkError = (): HttpNetworkError => error({ kind: ErrorKind.HttpNetworkError });
export const HttpUnknownError = (details?: unknown): HttpUnknownError =>
	error({ kind: ErrorKind.HttpUnknownError, details });
export const JsonSyntaxError = (): JsonSyntaxError => error({ kind: ErrorKind.JsonSyntaxError });
export const JsonUnknownError = (details?: unknown): JsonUnknownError =>
	error({ kind: ErrorKind.JsonUnknownError, details });
export const InternalServerError = (message?: string): InternalServerError =>
	error({ kind: ErrorKind.InternalServerError, message: message ?? '' });
export const MissingOAuthStateError = (): MissingOAuthStateError =>
	error({ kind: ErrorKind.MissingOAuthStateError });
export const MismatchOAuthStateError = (): MismatchOAuthStateError =>
	error({ kind: ErrorKind.MismatchOAuthStateError });
export const MissingGoogleAuthorizationCodeError = (): MissingGoogleAuthorizationCodeError =>
	error({ kind: ErrorKind.MissingGoogleAuthorizationCodeError });
export const ValidationError = Object.assign(
	(errors: Record<string, string[]>, detail?: string): ValidationError =>
		error({ kind: ErrorKind.ValidationError, errors, detail }),
	{
		from: (problemDetails: ProblemDetails) => {
			return ValidationError(
				problemDetails.errors?.reduce<Record<string, string[]>>((acc, cur) => {
					if (!acc[cur.field]) {
						acc[cur.field] = [cur.code];
					} else {
						acc[cur.field].push(cur.code);
					}
					return acc;
				}, {}) ?? {},
				problemDetails.detail
			);
		},
	}
);
export const GenericError = (context: unknown): GenericError =>
	error({ kind: ErrorKind.GenericError, context });
export function NotFoundError(): NotFoundError {
	return error({ kind: ErrorKind.NotFoundError });
}

export const enrich =
	(context: { step: string }) =>
	<T>(error: T): RichError<T> => {
		return {
			...error,
			context,
		};
	};

export const enrichStep = (step: string) => {
	return enrich({ step });
};

export const mapFetchException = (e: unknown) => {
	if (isNetworkError(e)) {
		return HttpNetworkError();
	}
	if (e instanceof Error) {
		if (e.name === 'AbortError') {
			return AbortError();
		}
		return HttpUnknownError({
			name: e.name,
			message: e.message,
		});
	}
	throw e;
};

export const mapJsonException = (e: unknown) => {
	if (e instanceof SyntaxError) {
		return JsonSyntaxError();
	}
	if (e instanceof Error && e.name === 'AbortError') {
		if (e.name === 'AbortError') {
			return AbortError();
		}
		return JsonUnknownError({
			name: e.name,
			mesage: e.message,
		});
	}
	throw e;
};

const errorSchema = v.object({
	kind: v.union(Object.values(ErrorKind).map((a) => v.literal(a))),
});
const errorValidator = createValidator(errorSchema);

const richErrorValidator = createValidator(
	v.object({
		context: v.object({
			step: v.string(),
		}),
	})
);

export const isError = (error: unknown): error is v.InferOutput<typeof errorSchema> => {
	return errorValidator.check(error);
};

export const isRichError = <T>(error: T): error is RichError<T> => {
	return richErrorValidator.check(error);
};
