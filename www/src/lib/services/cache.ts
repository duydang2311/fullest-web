import type { Cacheable } from 'cacheable';

export interface Cache {
	get<T>(key: string): Promise<T | undefined>;
	set(key: string, value: unknown, ttl?: number | string): Promise<boolean>;
	getOrCreate<T>(
		key: string,
		create: () => Promise<T>,
		options?: CacheGetOrCreateOptions
	): Promise<T>;
	getOrCreate<T>(key: string, create: () => T, options?: CacheGetOrCreateOptions): Promise<T>;
}

export interface CacheGetOrCreateOptions {
	ttl?: number | string;
	cacheErrors?: boolean;
	throwErrors?: boolean;
}

export class MemoryCache implements Cache {
	readonly #cacheable: Cacheable;

	constructor(cacheable: Cacheable) {
		this.#cacheable = cacheable;
	}

	public getOrCreate<T>(
		key: string,
		create: () => T | Promise<T>,
		options?: CacheGetOrCreateOptions
	) {
		return this.#cacheable.getOrSet(
			key,
			() => {
				const ret = create();
				if (ret instanceof Promise) {
					return ret;
				}
				return Promise.resolve(ret);
			},
			options
		);
	}

	public set(key: string, value: unknown, ttl?: number | string) {
		return this.#cacheable.set(key, value, ttl);
	}

	public get<T>(key: string) {
		return this.#cacheable.get<T>(key);
	}
}
