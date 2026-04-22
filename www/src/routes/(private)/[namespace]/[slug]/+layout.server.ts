import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import invariant from 'tiny-invariant';
import type { Project } from '~/lib/models/project';
import type { HttpClient } from '~/lib/services/http_client';
import { NotFoundError, traced } from '~/lib/utils/errors';
import { jsonify, parseHttpError } from '~/lib/utils/http';
import { useRuntime } from '~/lib/utils/runtime.server';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async (e) => {
    invariant(e.locals.session, 'session must not be null');

    const ns = await fetchNamespace(e.params.namespace);
    if (!ns.ok) {
        return error(500, ns.error);
    }

    const project = await makeFetchProject(e.locals.http)(ns.data.id, e.params.slug);
    if (!project.ok) {
        return error(500, project.error);
    }

    return {
        user: e.locals.session.user,
        project: project.data,
        namespace: ns.data,
        pathname: e.url.pathname
    };
};

async function fetchNamespace(namespace: string) {
    const { http } = useRuntime();
    const ns = await http.get(`namespaces/${namespace}`, {
        query: {
            fields: 'Id,Kind,User.Id,User.Name,User.DisplayName',
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
                            user: { id: string; name: string; displayName: string };
                        }
                    >()
                );
            }
            if (response.status === 404) {
                return error(404, NotFoundError());
            }
            const err = await parseHttpError(response);
            return attempt.fail(err);
        }),
        attempt.mapError(traced('fetch_namespace'))
    );
}

function makeFetchProject(http: HttpClient) {
    return async (namespaceId: string, identifier: string) => {
        const fetched = await http.get(`namespaces/${namespaceId}/${identifier}`, {
            query: { fields: 'Id' },
        });

        return fetched.pipe(
            attempt.flatMap(async (resp) => {
                if (!resp.ok) {
                    if (resp.status === 404) {
                        return error(404, NotFoundError());
                    }
                    const err = await parseHttpError(resp);
                    return attempt.fail(err);
                }
                return await jsonify(() => resp.json<Pick<Project, 'id'>>());
            }),
            attempt.mapError(traced('fetch_project'))
        );
    };
}
