import isNetworkError from 'is-network-error';
import * as v from 'valibot';
import type { ProblemDetails } from './problem';
import { createValidator } from './validation';

export enum ErrorKind {
	Abort = 'Abort',
	HttpNetwork = 'HttpNetwork',
	HttpUnknown = 'HttpUnknown',
	JsonSyntax = 'JsonSyntax',
	JsonUnknown = 'JsonUnknown',
	InternalServer = 'InternalServer',
	MissingOAuthState = 'MissingOAuthState',
	MismatchOAuthState = 'MismatchOAuthState',
	MissingGoogleAuthorizationCode = 'MissingGoogleAuthorizationCode',
	Validation = 'Validation',
	GoogleOAuthExchange = 'GoogleOAuthExchange',
	Generic = 'Generic',
	NotFound = 'NotFound',
	Forbidden = 'Forbidden',
	Unknown = 'Unknown',
}

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

export interface AbortError extends App.Error {
	kind: ErrorKind.Abort;
}

export interface HttpNetworkError extends App.Error {
	kind: ErrorKind.HttpNetwork;
}

export interface HttpUnknownError extends App.Error {
	kind: ErrorKind.HttpUnknown;
	details?: unknown;
}

export interface JsonSyntaxError extends App.Error {
	kind: ErrorKind.JsonSyntax;
}

export interface JsonUnknownError extends App.Error {
	kind: ErrorKind.JsonUnknown;
	details?: unknown;
}

export interface InternalServerError extends App.Error {
	kind: ErrorKind.InternalServer;
}

export interface MissingOAuthStateError extends App.Error {
	kind: ErrorKind.MissingOAuthState;
}

export interface MismatchOAuthStateError extends App.Error {
	kind: ErrorKind.MismatchOAuthState;
}

export interface MissingGoogleAuthorizationCodeError extends App.Error {
	kind: ErrorKind.MissingGoogleAuthorizationCode;
}

export interface ValidationError extends App.Error {
	kind: ErrorKind.Validation;
	errors: Record<string, string[]>;
	detail?: string;
}

export interface GenericError extends App.Error {
	kind: ErrorKind.Generic;
	context: unknown;
}

export interface NotFoundError extends App.Error {
	kind: ErrorKind.NotFound;
}

export interface ForbiddenError extends App.Error {
	kind: ErrorKind.Forbidden;
}

export interface UnknownError extends App.Error {
	kind: ErrorKind.Unknown;
	context?: unknown;
}

const error = <T>(object: T) => ({
	message: '', // bypassing App.Error required property
	...object,
});

type RichError<T> = T & { context: { step: string } };
export type Traced<T> = T & { trace: string };

export const AbortError = (): AbortError => error({ kind: ErrorKind.Abort });
export const HttpNetworkError = (): HttpNetworkError => error({ kind: ErrorKind.HttpNetwork });
export const HttpUnknownError = (details?: unknown): HttpUnknownError =>
	error({ kind: ErrorKind.HttpUnknown, details });
export const JsonSyntaxError = (): JsonSyntaxError => error({ kind: ErrorKind.JsonSyntax });
export const JsonUnknownError = (details?: unknown): JsonUnknownError =>
	error({ kind: ErrorKind.JsonUnknown, details });
export const InternalServerError = (message?: string): InternalServerError =>
	error({ kind: ErrorKind.InternalServer, message: message ?? '' });
export const MissingOAuthStateError = (): MissingOAuthStateError =>
	error({ kind: ErrorKind.MissingOAuthState });
export const MismatchOAuthStateError = (): MismatchOAuthStateError =>
	error({ kind: ErrorKind.MismatchOAuthState });
export const MissingGoogleAuthorizationCodeError = (): MissingGoogleAuthorizationCodeError =>
	error({ kind: ErrorKind.MissingGoogleAuthorizationCode });
export const ValidationError = Object.assign(
	(errors: Record<string, string[]>, detail?: string): ValidationError =>
		error({ kind: ErrorKind.Validation, errors, detail }),
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
	error({ kind: ErrorKind.Generic, context });
export function NotFoundError(): NotFoundError {
	return error({ kind: ErrorKind.NotFound });
}
export function ForbiddenError(): ForbiddenError {
	return error({ kind: ErrorKind.Forbidden });
}
export function UnknownError(context?: unknown): UnknownError {
	return error({ kind: ErrorKind.Unknown, context });
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

export function traced(trace: string) {
	return <T>(error: T): Traced<T> => {
		return {
			...error,
			trace,
		};
	};
}

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

export const flattenErrorEntries = ([field, errors]: [string, string[]]): string[] => {
	return errors.map((e) => `${field}:${e}`);
};
