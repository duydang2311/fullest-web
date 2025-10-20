import { attempt } from '@duydang2311/attempt';
import { error, fail, redirect } from '@sveltejs/kit';
import invariant from 'tiny-invariant';
import * as v from 'valibot';
import type { Task } from '~/lib/models/task';
import { enrichStep, ForbiddenError, GenericError, ValidationError } from '~/lib/utils/errors';
import { jsonify, parseHttpProblem } from '~/lib/utils/http';
import { createValidator } from '~/lib/utils/validation';
import type { Actions } from './$types';

export const actions: Actions = {
	default: async (e) => {
		const formData = await e.request.formData();
		const input = {
			projectId: formData.get('projectId'),
			title: formData.get('title'),
			descriptionJson: formData.get('description_json'),
			descriptionText: formData.get('description_text'),
		};
		invariant(typeof input.projectId === 'string', 'projectId must be a string');

		const validated = validator.parse(input);
		if (!validated.ok) {
			return fail(400, validated.error);
		}

		if (validated.data.descriptionJson) {
			validated.data.descriptionJson = JSON.parse(validated.data.descriptionJson);
		}
		const response = await e.locals.http
			.post(`projects/${formData.get('projectId')}/tasks`, {
				body: validated.data,
			})
			.then(attempt.unwrapOrElse((e) => error(500, enrichStep('create_project')(e))));
		if (!response.ok) {
			switch (response.status) {
				case 403:
					return fail(403, ForbiddenError());
				case 400: {
					const problem = (await jsonify(() => response.json())).pipe(
						attempt.flatMap((body) => parseHttpProblem(body)),
						attempt.unwrapOrElse((e) => error(500, enrichStep('parse_problem')(e)))
					);
					return fail(response.status, ValidationError.from(problem));
				}
				default:
					return error(
						response.status,
						GenericError(
							(await jsonify(() => response.json())).pipe(attempt.unwrapOrElse((e) => e))
						)
					);
			}
		}

		const task = await jsonify(() => response.json<Pick<Task, 'id' | 'publicId'>>()).then(
			attempt.unwrapOrElse((e) => error(500, enrichStep('jsonify')(e)))
		);
		const path = `/${e.params.namespace}/${e.params.slug}/tasks/${task.publicId}`;
		e.cookies.set('last_created_task_id', task.id, {
			path,
			secure: true,
			httpOnly: true,
			sameSite: 'lax',
		});
		return redirect(302, path);
	},
};

const validator = createValidator(
	v.object({
		projectId: v.string(),
		title: v.string(),
		descriptionJson: v.pipe(v.nullish(v.string())),
		descriptionText: v.nullish(v.string()),
	})
);
