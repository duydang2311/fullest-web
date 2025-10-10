import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import type { OffsetList } from '~/lib/models/paginated';
import type { Status } from '~/lib/models/status';
import type { Task } from '~/lib/models/task';
import { enrichStep, ErrorKind, ForbiddenError, traced, UnknownError } from '~/lib/utils/errors';
import { jsonify } from '~/lib/utils/http';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
	const parent = await e.parent();
	const [statusList, fetchedTasks] = await Promise.all([
		e.locals.http
			.get('statuses', {
				query: { projectId: parent.project.id, fields: 'Id,Name,Rank,Category', sort: '-Rank' },
			})
			.then((att) =>
				att.pipe(
					attempt.flatMap((response) =>
						jsonify(() =>
							response.json<OffsetList<Pick<Status, 'id' | 'name' | 'rank' | 'category'>>>()
						)
					),
					attempt.unwrapOrElse((e) => error(500, enrichStep('fetch_statuses')(e)))
				)
			),
		await e.locals.http
			.get('tasks', {
				query: {
					projectId: parent.project.id,
					fields: 'Id,PublicId,Title',
					sort: '-Id',
				},
			})
			.then((att) =>
				att.pipe(
					attempt.flatMap(async (response) =>
						response.ok
							? await jsonify(() =>
									response.json<OffsetList<Pick<Task, 'id' | 'publicId' | 'title'>>>()
								)
							: response.status === 403
								? attempt.fail(ForbiddenError())
								: attempt.fail(UnknownError(await response.text()))
					)
				)
			),
	]);

	if (!fetchedTasks.ok) {
		return error(
			fetchedTasks.error.kind === ErrorKind.Forbidden ? 403 : 500,
			traced('fetch_tasks')(fetchedTasks.error)
		);
	}

	return {
		statuses: statusList,
		taskList: fetchedTasks.data,
	};
};

// for grouped list view
// export const load: PageServerLoad = async (e) => {
// 	const parent = await e.parent();
// 	const [statuses, fetchedTasks] = await Promise.all([
// 		e.locals.http
// 			.get('statuses', {
// 				query: { projectId: parent.project.id, fields: 'Id,Name,Rank,Category', sort: '-Rank' },
// 			})
// 			.then((att) =>
// 				att.pipe(
// 					attempt.flatMap((response) =>
// 						jsonify(() =>
// 							response.json<Paginated<Pick<Status, 'id' | 'name' | 'rank' | 'category'>>>()
// 						)
// 					),
// 					attempt.unwrapOrElse((e) => error(500, enrichStep('fetch_statuses')(e)))
// 				)
// 			),
// 		await e.locals.http
// 			.get('tasks/group-by/status', {
// 				query: {
// 					projectId: parent.project.id,
// 					fields: 'Id,PublicId,Title,StatusId',
// 					sort: '-Id',
// 				},
// 			})
// 			.then((att) =>
// 				att.pipe(
// 					attempt.flatMap(async (response) =>
// 						response.ok
// 							? await jsonify(() =>
// 									response.json<Paginated<Pick<Task, 'id' | 'publicId' | 'title' | 'statusId'>>[]>()
// 								)
// 							: response.status === 403
// 								? attempt.fail(ForbiddenError())
// 								: attempt.fail(UnknownError(await response.text()))
// 					)
// 				)
// 			),
// 	]);

// 	if (!fetchedTasks.ok) {
// 		return error(
// 			fetchedTasks.error.kind === ErrorKind.Forbidden ? 403 : 500,
// 			traced('fetch_tasks')(fetchedTasks.error)
// 		);
// 	}

// 	return {
// 		statuses,
// 		taskLists: fetchedTasks.data,
// 	};
// };
