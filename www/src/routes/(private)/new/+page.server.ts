import { error, fail, redirect } from '@sveltejs/kit';
import invariant from 'tiny-invariant';
import * as v from 'valibot';
import { enrichStep, GenericError, ValidationError } from '~/lib/utils/errors';
import { jsonify, parseHttpProblem } from '~/lib/utils/http';
import { useRuntime } from '~/lib/utils/runtime';
import { createValidator } from '~/lib/utils/validation';
import type { Actions, PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
	invariant(e.locals.session, 'session must not be null');
	return {
		username: e.locals.session.user.name,
	};
};

export const actions: Actions = {
	default: async (e) => {
		const { http } = useRuntime();
		const validated = validator.parse(decode(await e.request.formData()));
		if (!validated.ok) {
			return fail(400, validated.error);
		}

		const created = await http.post('projects', { body: validated.data });
		if (!created.ok) {
			return fail(created.error.kind === 'HttpNetworkError' ? 502 : 500, created.error);
		}
		if (!created.data.ok) {
			const parsedBody = await jsonify(() => created.data.json());
			if (!parsedBody.ok) {
				return error(500, parsedBody.error);
			}

			const parsedProblem = parseHttpProblem(parsedBody.data);
			if (!parsedProblem.ok) {
				return error(500, GenericError(parsedBody.data));
			}
			return fail(created.data.status, ValidationError.from(parsedProblem.data));
		}

		const parsedBody = await jsonify(() =>
			created.data.json<{
				id: string;
				identifier: string;
			}>()
		);
		if (!parsedBody.ok) {
			return fail(500, enrichStep('parse_successs_body')(parsedBody.error));
		}

		const { session } = useRuntime();
		invariant(session, 'session must not be null');

		const path = `/${session.user.name}/${parsedBody.data.identifier}`;
		e.cookies.set('last_created_project', parsedBody.data.id, {
			path,
			httpOnly: true,
			secure: true,
			sameSite: 'lax',
		});

		return redirect(303, path);
	},
};

const validator = createValidator(
	v.object({
		name: v.string(),
		identifier: v.string(),
		summary: v.optional(v.string()),
	})
);

function decode(formData: FormData) {
	return {
		name: formData.get('name'),
		identifier: formData.get('identifier'),
		summary: formData.get('summary'),
	};
}
