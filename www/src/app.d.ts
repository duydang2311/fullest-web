// See https://svelte.dev/docs/kit/types#app.d.ts

import type { Cache } from '$lib/services/cache';
import type { HttpClient } from '$lib/services/http_client';

// for information about these interfaces
declare global {
	type MaybePromise<T> = T | Promise<T>;

	namespace App {
		interface Error {
			kind: string;
			message: string;
		}
		interface Locals {
			http: HttpClient;
			cache: Cache;
		}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}

	interface Body {
		json<T>(): Promise<T>;
	}
}

export { };

