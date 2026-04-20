import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import type { Project } from '~/lib/models/project';
import type { HttpClient } from '~/lib/services/http_client';
import { NotFoundError, traced } from '~/lib/utils/errors';
import { jsonify, parseHttpError } from '~/lib/utils/http';
import type { PageServerLoad } from './$types';
import sanitize from 'sanitize-html';
import { renderToHTMLString } from '~/lib/components/editor.svelte';

export const load: PageServerLoad = async (e) => {
    const parent = await e.parent();
    const project = await makeFetchProject(e.locals.http)(parent.namespace.id, e.params.slug);
    if (!project.ok) {
        if (project.error.kind === 'HttpError' || project.error.kind === 'HttpValidationError') {
            return error(project.error.status, project.error);
        }
        return error(500, project.error);
    }

    const isCreatedJustNow = e.cookies.get('last_created_project') === project.data.id;
    if (isCreatedJustNow) {
        e.cookies.delete('last_created_project', {
            path: e.untrack(() => e.url.pathname),
            httpOnly: true,
            secure: true,
            sameSite: 'lax',
        });
    }

    if (project.data.descriptionJson) {
        project.data.descriptionHtml = sanitize(
            renderToHTMLString(JSON.parse(project.data.descriptionJson))
        );
    }

    return {
        project: project.data,
        isCreatedJustNow,
    };
};

function makeFetchProject(http: HttpClient) {
    return async (namespaceId: string, identifier: string) => {
        const fetched = await http.get(`namespaces/${namespaceId}/${identifier}`, {
            query: { fields: 'Id,Name,Identifier,Summary,DescriptionJson,Tags' },
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
                return await jsonify(() =>
                    resp.json<
                        Pick<
                            Project,
                            'id' | 'name' | 'identifier' | 'summary' | 'descriptionJson' | 'tags'
                        > & { descriptionHtml: string | null }
                    >()
                );
            }),
            attempt.mapError(traced('fetch_project'))
        );
    };
}
