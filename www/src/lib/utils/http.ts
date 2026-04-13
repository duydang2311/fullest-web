import { attempt } from '@duydang2311/attempt';
import { HttpErr, HttpValidationErr, mapJsonException } from './errors';
import { guard } from './guard';
import { problemDetailsValidator } from './problem';

export type Direction = 'asc' | 'desc';

export const jsonify = <T>(f: () => Promise<T>) => {
    return attempt.async(() => f())(mapJsonException);
};

export function parseHttpProblem(input: Response | unknown) {
    return problemDetailsValidator.parse(input);
}

export async function parseHttpError(response: Response) {
    guard(!response.ok, 'response must have error status code');
    if (response.status >= 500) {
        return HttpErr(response.status);
    }

    const contentType = response.headers.get('Content-Type');
    if (
        !contentType ||
        (!contentType.includes('application/json') &&
            !contentType.includes('application/problem+json'))
    ) {
        return HttpErr(response.status);
    }

    const parsedBody = await jsonify(() => response.json());
    if (!parsedBody.ok) {
        return HttpErr(response.status);
    }

    const result = parseHttpProblem(parsedBody.data);
    if (!result.ok) {
        return HttpErr(response.status);
    }
    if (!result.data.errors) {
        return HttpErr(response.status);
    }
    return HttpValidationErr(
        response.status,
        result.data.errors.reduce(
            (acc, cur) => {
                if (!acc[cur.field]) {
                    acc[cur.field] = [cur.code];
                } else {
                    acc[cur.field].push(cur.code);
                }
                return acc;
            },
            {} as Record<string, string[]>
        )
    );
}

export function fields(...values: (string | Record<string, string>)[]) {
    const set = new Set<string>();
    for (const value of values) {
        if (typeof value === 'string') {
            for (const split of value.split(',')) {
                set.add(split);
            }
        } else {
            for (const [k, v] of Object.entries(value)) {
                for (const split of v.split(',')) {
                    set.add(`${k}.${split}`);
                }
            }
        }
    }
    return Array.from(set).toSorted().join(',');
}
