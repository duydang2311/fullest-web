import { attempt } from '@duydang2311/attempt';
import { mapJsonException } from './errors';
import { problemDetailsValidator } from './problem';

export const jsonify = <T>(f: () => Promise<T>) => {
	return attempt.async(() => f())(mapJsonException);
};

export function parseHttpProblem(input: Response | unknown) {
	return problemDetailsValidator.parse(input);
}
