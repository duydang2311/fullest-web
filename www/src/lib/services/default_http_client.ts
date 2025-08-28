import { mapFetchException } from '$lib/utils/errors';
import { trimEnd, trimStart } from '$lib/utils/string';
import { attempt } from '@duydang2311/attempt';
import type { HttpClient, HttpRequestOptions } from './http_client';
import { isPlainObject } from 'is-what';

export interface CreateDefaultHttpClientOptions {
	fetcher: (input: RequestInfo | URL, init?: RequestInit) => Promise<Response>;
	prefix?: string;
	suffix?: string;
	headers?: HeadersInit;
}

export class DefaultHttpClient implements HttpClient {
	#options: CreateDefaultHttpClientOptions;

	constructor(options: CreateDefaultHttpClientOptions) {
		this.#options = options;
	}

	public async fetchRaw(url: string, options?: HttpRequestOptions) {
		let headers = this.#mergeHeaders(this.#options.headers, options?.headers);
		if (options?.body && (isPlainObject(options.body) || Array.isArray(options.body))) {
			options.body = JSON.stringify(options.body);
			if (headers == null) {
				headers = { 'Content-Type': 'application/json' };
			} else if (headers['Content-Type'] == null) {
				headers['Content-Type'] = 'application/json';
			}
		}
		return this.#options.fetcher(this.#normalizeUrl(url), {
			...options,
			headers,
		} as RequestInit);
	}

	public async fetch(url: string, options?: HttpRequestOptions) {
		const fetched = await attempt.async(() => this.fetchRaw(url, options))(mapFetchException);
		if (fetched.failed) {
			return attempt.fail(fetched.error);
		}
		return attempt.ok(fetched.data);
	}

	public get(url: string, options?: HttpRequestOptions) {
		return this.fetch(url, { ...options, method: 'GET' });
	}

	public delete(url: string, options?: HttpRequestOptions) {
		return this.fetch(url, { ...options, method: 'DELETE' });
	}

	public post(url: string, options?: HttpRequestOptions) {
		return this.fetch(url, { ...options, method: 'POST' });
	}

	public put(url: string, options?: HttpRequestOptions) {
		return this.fetch(url, { ...options, method: 'PUT' });
	}

	public patch(url: string, options?: HttpRequestOptions) {
		return this.fetch(url, { ...options, method: 'PATCH' });
	}

	readonly #slash = '/'.charCodeAt(0);
	#normalizeUrl(url: string) {
		if (this.#options.prefix != null) {
			url = trimEnd(this.#options.prefix, this.#slash) + '/' + trimStart(url, this.#slash);
		}
		if (this.#options.suffix != null) {
			url = trimEnd(url, this.#slash) + '/' + trimStart(this.#options.suffix, this.#slash);
		}
		return url;
	}

	#mergeHeaders(a?: HeadersInit, b?: HeadersInit) {
		if (a == null && b == null) {
			return undefined;
		}
		return Object.assign(
			{},
			a instanceof Headers
				? Object.fromEntries(a.entries())
				: Array.isArray(a)
					? Object.fromEntries(a)
					: a,
			b instanceof Headers
				? Object.fromEntries(b.entries())
				: Array.isArray(b)
					? Object.fromEntries(b)
					: b
		);
	}
}
