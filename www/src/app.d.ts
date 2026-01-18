// See https://svelte.dev/docs/kit/types#app.d.ts

import type { Cache } from '$lib/services/cache';
import type { HttpClient } from '$lib/services/http_client';
import 'unplugin-icons/types/svelte';
import type { UserPreset } from './lib/models/user';

// for information about these interfaces
declare global {
	type MaybePromise<T> = T | Promise<T>;
	type EitherOr<A, B> =
		| (A & { [K in Exclude<keyof B, keyof A>]?: never })
		| (B & { [K in Exclude<keyof A, keyof B>]?: never });

	namespace App {
		interface Error {
			kind: string;
			message: string;
		}
		interface Locals {
			http: HttpClient;
			cache: Cache;
			session?: { user: Pick<User, 'id'> & UserPreset['Avatar'] };
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

