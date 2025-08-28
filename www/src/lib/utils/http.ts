import { attempt } from '@duydang2311/attempt';
import { mapJsonException } from './errors';
import { problemDetailsValidator } from './problem';

export const jsonify = <T>(f: () => Promise<T>) => {
	return attempt.async(() => f())(mapJsonException);
};

export const parseHttpProblem = async (response: Response) => {
	const contentType = response.headers.get('Content-Type');
	if (
		!contentType ||
		!(contentType.includes('application/json') || contentType.includes('application/problem+json'))
	) {
		throw new Error('Unknown HTTP error response format');
	}

	const parsed = await jsonify(() => response.json());
	if (parsed.failed) {
		return parsed;
	}

	return problemDetailsValidator.parse(parsed.data);
};
