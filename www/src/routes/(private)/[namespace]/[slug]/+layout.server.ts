import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import invariant from 'tiny-invariant';
import type { Project } from '~/lib/models/project';
import { enrichStep, NotFoundError } from '~/lib/utils/errors';
import { jsonify, parseHttpError } from '~/lib/utils/http';
import { useRuntime } from '~/lib/utils/runtime.server';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async (e) => {
    invariant(e.locals.session, 'session must not be null');

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
        user: e.locals.session.user,
        namespace: fetchedAll.data.namespace,
        project: fetchedAll.data.project,
        isCreatedJustNow,
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
        const err = await parseHttpError(fetched.data);
        return attempt.fail(err);
    }

    return await jsonify(() =>
        fetched.data.json<
            Pick<Project, 'id' | 'name' | 'identifier' | 'summary' | 'about' | 'tags'>
        >()
    );
}
