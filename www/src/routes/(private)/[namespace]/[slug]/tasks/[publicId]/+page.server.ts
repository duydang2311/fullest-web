import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import type { Task } from '~/lib/models/task';
import { ErrorKind, ForbiddenError, NotFoundError, UnknownError } from '~/lib/utils/errors';
import { jsonify } from '~/lib/utils/http';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
	const parent = await e.parent();
	const fetchedTask = await e.locals.http.get(
		`projects/${parent.project.id}/tasks/${e.params.publicId}`,
		{
			query: { fields: 'Id,PublicId,Title,Description,Status,Priority,CreatedTime,UpdatedTime' },
		}
	);

	const task = await fetchedTask.pipe(
		attempt.flatMap(async (response) => {
			if (response.status === 403) {
				return attempt.fail(ForbiddenError());
			}
			if (response.status === 404) {
				return attempt.fail(NotFoundError());
			}
			if (!response.ok) {
				return attempt.fail(UnknownError(await response.text()));
			}
			return jsonify(() =>
				response.json<
					Pick<
						Task,
						| 'id'
						| 'publicId'
						| 'title'
						| 'description'
						| 'status'
						| 'priority'
						| 'createdTime'
						| 'updatedTime'
					>
				>()
			);
		}),
		attempt.unwrapOrElse((e) => {
			return error(
				e.kind === ErrorKind.Forbidden ? 403 : e.kind === ErrorKind.NotFound ? 404 : 500,
				e
			);
		})
	);

	return { task };
};
