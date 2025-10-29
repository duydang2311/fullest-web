import { command, getRequestEvent } from '$app/server';
import { attempt } from '@duydang2311/attempt';
import { BadHttpResponse } from '../utils/errors';
import { jsonify } from '../utils/http';

export const createProfilePictureUploadRequest = command('unchecked', async () => {
	const e = getRequestEvent();
	const created = await e.locals.http.post('assets/profile-upload-request');
	if (!created.ok) {
		return created;
	}
	if (!created.data.ok) {
		return attempt.fail(BadHttpResponse(created.data.status));
	}

	const parsed = await jsonify(() =>
		created.data.json<{
			accessToken: string;
			key: string;
		}>()
	);
	return parsed;
});
