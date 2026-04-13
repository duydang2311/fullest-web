export function guardNull(condition: any, name?: string): asserts condition {
    if (condition) {
        return;
    }
    throw new Error(`${name ?? 'value'} must not be null`);
}
