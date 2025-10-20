import { command, getRequestEvent } from '$app/server';
import { attempt } from '@duydang2311/attempt';
import * as v from 'valibot';
import { BadHttpResponse } from '../utils/errors';

export const editComment = command(
	v.object({
		id: v.string(),
		contentJson: v.nullish(v.string()),
	}),
	async (data) => {
		const e = getRequestEvent();
		return (
			await e.locals.http.patch(`comments/${data.id}`, {
				body: {
					patch: {
						contentJson: data.contentJson,
					},
				},
			})
		).pipe(
			attempt.flatMap(async (response) => {
				if (response.ok) {
					return attempt.ok<void>(void 0);
				}
				return attempt.fail(BadHttpResponse(response.status, await response.text()));
			})
		);
	}
);

export const deleteComment = command(
	v.object({
		id: v.string(),
	}),
	async (data) => {
		const e = getRequestEvent();
		return (await e.locals.http.delete(`comments/${data.id}`)).pipe(
			attempt.flatMap(async (response) => {
				if (response.ok) {
					return attempt.ok<void>(void 0);
				}
				return attempt.fail(BadHttpResponse(response.status, await response.text()));
			})
		);
	}
);
