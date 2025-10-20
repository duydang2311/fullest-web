export interface OffsetList<T> {
	items: T[];
	page: number;
	size: number;
	totalItems: number;
	totalPages: number;
}

export function offsetList<T>(): OffsetList<T>;
export function offsetList<T>(
	items: T[],
	page: number,
	size: number,
	totalItems: number,
	totalPages: number
): OffsetList<T>;
export function offsetList<T>(
	items?: T[],
	page?: number,
	size?: number,
	totalItems?: number,
	totalPages?: number
): OffsetList<T> {
	return {
		items: items ?? [],
		page: page ?? 0,
		size: size ?? 0,
		totalItems: totalItems ?? 0,
		totalPages: totalPages ?? 0,
	};
}
