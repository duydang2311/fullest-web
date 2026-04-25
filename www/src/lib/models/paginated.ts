export interface OffsetList<T> {
    items: T[];
    page: number;
    size: number;
    totalCount: number;
    totalPages: number;
}

export interface KeysetList<T> {
    items: T[];
    hasPrevious: boolean;
    hasNext: boolean;
    totalCount: number;
}

export interface CursorList<T, TKey> {
    items: T[];
    cursor: TKey | null;
    hasMore: boolean;
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

export function keysetList<T>(): KeysetList<T>;
export function keysetList<T>(options: {
    items?: T[];
    hasPrevious?: boolean;
    hasNext?: boolean;
    totalCount?: number;
}): KeysetList<T>;
export function keysetList<T>(options?: {
    items?: T[];
    hasPrevious?: boolean;
    hasNext?: boolean;
    totalCount?: number;
}): KeysetList<T> {
    if (options == null) {
        return {
            items: [],
            hasPrevious: false,
            hasNext: false,
            totalCount: 0,
        };
    }
    return {
        items: options.items ?? [],
        hasPrevious: options.hasPrevious ?? false,
        hasNext: options.hasNext ?? false,
        totalCount: options.totalCount ?? 0,
    };
}

export function reverseKeysetList<T>(list: KeysetList<T>) {
    return keysetList({
        items: list.items.toReversed(),
        hasPrevious: list.hasNext,
        hasNext: list.hasPrevious,
        totalCount: list.totalCount,
    });
}

export function cursorList<T, TKey>(): CursorList<T, TKey>;
export function cursorList<T, TKey>(
    items: T[],
    hasMore: boolean,
    cursor?: TKey
): CursorList<T, TKey>;
export function cursorList<T, TKey>(
    items?: T[],
    hasMore?: boolean,
    cursor?: TKey
): CursorList<T, TKey> {
    return {
        items: items ?? [],
        cursor: cursor ?? null,
        hasMore: hasMore ?? false,
    };
}

export function offsetPagination({ list, page }: { list: OffsetList<unknown>; page: number }) {
    let pages: (number | '...')[] = undefined!;
    if (list.totalPages <= 7) {
        pages = Array.from({ length: list.totalPages }, (_, i) => i + 1);
    } else {
        if (page <= 4) {
            pages = [1, 2, 3, 4, 5, '...', list.totalPages];
        } else if (page >= list.totalPages - 3) {
            pages = [
                1,
                '...',
                list.totalPages - 4,
                list.totalPages - 3,
                list.totalPages - 2,
                list.totalPages - 1,
                list.totalPages,
            ];
        } else {
            pages = [1, '...', page - 1, page, page + 1, '...', list.totalPages];
        }
    }
    return {
        page,
        pages,
        canPrev: page > 1,
        canNext: page < list.totalPages,
    };
}
