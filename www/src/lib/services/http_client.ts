import { AbortError, HttpNetworkError, HttpUnknownError } from '$lib/utils/errors';
import { type Attempt } from '@duydang2311/attempt';

export type HttpRequestOptions = Omit<RequestInit, 'body'> & {
	body?: Record<string, unknown> | Array<unknown> | BodyInit | null;
	query?: Record<string, number | string>;
};

export interface HttpClient {
	fetchRaw(url: string, options?: HttpRequestOptions): Promise<Response>;
	fetch(
		url: string,
		options?: HttpRequestOptions
	): Promise<Attempt<Response, AbortError | HttpNetworkError | HttpUnknownError>>;
	get(
		url: string,
		options?: HttpRequestOptions
	): Promise<Attempt<Response, AbortError | HttpNetworkError | HttpUnknownError>>;
	post(
		url: string,
		options?: HttpRequestOptions
	): Promise<Attempt<Response, AbortError | HttpNetworkError | HttpUnknownError>>;
	put(
		url: string,
		options?: HttpRequestOptions
	): Promise<Attempt<Response, AbortError | HttpNetworkError | HttpUnknownError>>;
	patch(
		url: string,
		options?: HttpRequestOptions
	): Promise<Attempt<Response, AbortError | HttpNetworkError | HttpUnknownError>>;
	delete(
		url: string,
		options?: HttpRequestOptions
	): Promise<Attempt<Response, AbortError | HttpNetworkError | HttpUnknownError>>;
}
