import { attempt } from '@duydang2311/attempt';
import invariant from 'tiny-invariant';
import { mapJsonException, ValidationError } from './errors';
import { problemDetailsValidator } from './problem';

export const jsonify = <T>(f: () => Promise<T>) => {
	return attempt.async(() => f())(mapJsonException);
};

export function parseHttpProblem(input: Response | unknown) {
	return problemDetailsValidator.parse(input);
}

export async function parseFailedResponse(response: Response) {
	invariant(!response.ok, 'response must have error status code');
	const parsedBody = await jsonify(() => response.json());
	if (!parsedBody.ok) {
		return parsedBody;
	}
	const parsedProblem = parseHttpProblem(parsedBody.data);
	if (!parsedProblem.ok) {
		return attempt.fail(ValidationError({ $: [response.status + ''] }));
	}
	return attempt.ok(parsedProblem.data);
}
