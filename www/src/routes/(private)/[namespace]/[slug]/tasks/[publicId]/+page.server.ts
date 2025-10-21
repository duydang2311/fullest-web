import { attempt } from '@duydang2311/attempt';
import { error, fail } from '@sveltejs/kit';
import type { Comment } from '~/lib/models/comment';
import { keysetList, type KeysetList } from '~/lib/models/paginated';
import type { Task } from '~/lib/models/task';
import type { User } from '~/lib/models/user';
import {
	BadHttpResponse,
	enrichStep,
	ErrorKind,
	ForbiddenError,
	NotFoundError,
	UnknownError,
	ValidationError,
} from '~/lib/utils/errors';
import { jsonify, parseHttpProblem } from '~/lib/utils/http';
import { useRuntime } from '~/lib/utils/runtime';
import { v } from '~/lib/utils/valibot';
import { createValidator } from '~/lib/utils/validation';
import type { Actions, PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
	e.depends(e.route.id);
	const parent = await e.parent();
	const fetchedTask = await e.locals.http.get(
		`projects/${parent.project.id}/tasks/${e.params.publicId}`,
		{
			query: {
				fields:
					'Id,PublicId,Title,Status,Priority,CreatedTime,UpdatedTime,InitialCommentId,Author.Name',
			},
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
						| 'status'
						| 'priority'
						| 'createdTime'
						| 'updatedTime'
						| 'initialCommentId'
					> & {
						author: Pick<User, 'name'>;
					}
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

	return {
		task,
		commentList: (await fetchComments(task.id)).pipe(
			attempt.unwrapOrElse(() => keysetList<never, never>())
		),
	};
};

async function fetchComments(taskId: string) {
	const { http } = useRuntime();
	return (
		await http.get('comments/keyset', {
			query: {
				taskId,
				fields: 'Id,ContentJson,CreatedTime,Author.Name',
				sort: 'Id',
				size: 20,
			},
		})
	).pipe(
		attempt.flatMap((response) =>
			jsonify(() =>
				response.json<
					KeysetList<
						Pick<Comment, 'id' | 'contentJson' | 'createdTime'> & { author: Pick<User, 'name'> },
						string
					>
				>()
			)
		)
	);
}

export const actions: Actions = {
	default: async (e) => {
		const formData = await e.request.formData();
		const created = await validator
			.parse({
				taskId: formData.get('taskId'),
				contentJson: formData.get('contentJson'),
				contentText: formData.get('contentText'),
			})
			.pipe(
				attempt.map((data) => ({
					...data,
					contentJson: data.contentJson ? JSON.parse(data.contentJson) : null,
				})),
				attempt.flatMap((data) =>
					e.locals.http.post('comments', {
						body: data,
					})
				)
			);
		if (!created.ok) {
			if (created.error.kind === ErrorKind.Validation) {
				return fail(400, created.error);
			}
			return error(500, created.error);
		}

		const response = created.data;
		if (!response.ok) {
			if (response.status === 400) {
				const parsed = (await jsonify(() => response.json())).pipe(
					attempt.flatMap(parseHttpProblem),
					attempt.map(ValidationError.from)
				);
				if (parsed.ok) {
					return fail(400, parsed.data);
				}
				return fail(500, enrichStep('parse_problem')(parsed.error));
			}
			return error(response.status, BadHttpResponse(response.status, await response.text()));
		}
		return { success: true };
	},
};

const validator = createValidator(
	v.object({
		taskId: v.string(),
		contentJson: v.nullish(v.string()),
		contentText: v.nullish(v.string()),
	})
);
