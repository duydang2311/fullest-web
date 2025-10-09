export function matchAll<T>(...values: T[]) {
	return (value: T) => values.includes(value);
}

export function matchNone<T>(...values: T[]) {
	return (value: T) => !values.includes(value);
}
