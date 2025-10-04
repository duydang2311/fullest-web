import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import { enrichStep, NotFoundError, ValidationError } from '~/lib/utils/errors';
import { jsonify, parseFailedResponse } from '~/lib/utils/http';
import { useRuntime } from '~/lib/utils/runtime';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
	const fetchedAll = await fetchNamespace(e.params.namespace).then((ns) =>
		ns.pipe(
			attempt.mapError((e) => enrichStep('fetch_namespace')(e)),
			attempt.flatMap((ns) =>
				fetchProject(ns.id, e.params.slug).then((att) =>
					att.pipe(
						attempt.mapError((e) => enrichStep('fetch_project')(e)),
						attempt.map((project) => ({ namespace: ns, project }))
					)
				)
			)
		)
	);
	if (!fetchedAll.ok) {
		return error(500, fetchedAll.error);
	}

	const isCreatedJustNow = e.cookies.get('last_created_project') === fetchedAll.data.project.id;
	if (isCreatedJustNow) {
		e.cookies.delete('last_created_project', {
			path: e.untrack(() => e.url.pathname),
			httpOnly: true,
			secure: true,
			sameSite: 'lax',
		});
	}
	return {
		namespace: fetchedAll.data.namespace,
		project: fetchedAll.data.project,
		isCreatedJustNow,
	};
};

async function fetchNamespace(namespace: string) {
	const { http } = useRuntime();
	const ns = await http.get(`namespaces/${namespace}`, {
		query: {
			fields: 'Id,Kind,User.Id,User.Name',
		},
	});
	return await ns.pipe(
		attempt.flatMap(async (response) => {
			if (response.ok) {
				return await jsonify(() =>
					response.json<
						{
							id: string;
						} & {
							kind: 'user';
							user: { id: string; name: string };
						}
					>()
				);
			}
			if (response.status === 404) {
				return error(404, NotFoundError());
			}
			const parsed = await parseFailedResponse(response);
			if (!parsed.ok) {
				return attempt.fail(parsed.error);
			}
			return attempt.fail(ValidationError.from(parsed.data));
		})
	);
}

async function fetchProject(namespaceId: string, identifier: string) {
	const { http } = useRuntime();
	const fetched = await http.get(`namespaces/${namespaceId}/${identifier}`, {
		query: { fields: 'Id,Name,Identifier,Summary,About,Tags' },
	});
	if (!fetched.ok) {
		return fetched;
	}

	if (!fetched.data.ok) {
		if (fetched.data.status === 404) {
			return error(404, NotFoundError());
		}
		const parsedFailedResponse = await parseFailedResponse(fetched.data);
		if (!parsedFailedResponse.ok) {
			return attempt.fail(parsedFailedResponse.error);
		}
		return attempt.fail(ValidationError.from(parsedFailedResponse.data));
	}

	return await jsonify(() =>
		fetched.data.json<{
			id: string;
			name: string;
			identifier: string;
			summary?: string;
			about?: string;
		}>()
	);
}
