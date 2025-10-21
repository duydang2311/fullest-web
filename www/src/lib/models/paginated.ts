export interface OffsetList<T> {
	items: T[];
	page: number;
	size: number;
	totalCount: number;
	totalPages: number;
}

export interface KeysetList<T, TKey> {
	items: T[];
	hasPrevious: boolean;
	hasNext: boolean;
	before?: TKey;
	after?: TKey;
	totalCount: number;
}

export function offsetList<T>(): OffsetList<T>;
export function offsetList<T>(
	items: T[],
	page: number,
	size: number,
	totalCount: number,
	totalPages: number
): OffsetList<T>;
export function offsetList<T>(
	items?: T[],
	page?: number,
	size?: number,
	totalCount?: number,
	totalPages?: number
): OffsetList<T> {
	return {
		items: items ?? [],
		page: page ?? 0,
		size: size ?? 0,
		totalCount: totalCount ?? 0,
		totalPages: totalPages ?? 0,
	};
}

export function keysetList<T, TKey>(): KeysetList<T, TKey>;
export function keysetList<T, TKey>(
	items: T[],
	totalCount: number,
	hasPrevious: boolean,
	hasNext: boolean,
	before?: TKey,
	after?: TKey
): KeysetList<T, TKey>;
export function keysetList<T, TKey>(
	items?: T[],
	totalCount?: number,
	hasPrevious?: boolean,
	hasNext?: boolean,
	before?: TKey,
	after?: TKey
): KeysetList<T, TKey> {
	return {
		items: items ?? [],
		totalCount: totalCount ?? 0,
		hasPrevious: hasPrevious ?? false,
		hasNext: hasNext ?? false,
		before,
		after,
	};
}
