import { command, getRequestEvent, query, requested } from '$app/server';
import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import { type } from 'arktype';
import sanitize from 'sanitize-html';
import { renderToHTMLString } from '~/lib/components/editor.svelte';
import type { Project } from '~/lib/models/project';
import type { HttpClient } from '~/lib/services/http_client';
import { traced } from '~/lib/utils/errors';
import { fields, jsonify, parseHttpError } from '~/lib/utils/http';

export const getProject = query(type({ id: 'string' }), async (data) => {
    const e = getRequestEvent();
    const result = await makeFetchProject(e.locals.http)(data.id);
    if (!result.ok) {
        return error(
            result.error.kind === 'HttpError' || result.error.kind === 'HttpValidationError'
                ? result.error.status
                : 500,
            result.error
        );
    }
    if (result.data.descriptionJson) {
        result.data.descriptionHtml = sanitize(
            renderToHTMLString(JSON.parse(result.data.descriptionJson))
        );
    }
    return result.data;
});

export const editDescription = command(
    type({
        projectId: 'string',
        descriptionJson: 'string | null',
        version: 'number',
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.patch(`projects/${data.projectId}`, {
            body: {
                patch: {
                    descriptionJson: data.descriptionJson || null,
                },
                version: data.version,
            },
        });
        if (!result.ok) {
            return attempt.fail(result.error);
        }

        const resp = result.data;
        if (!resp.ok) {
            return attempt.fail(await parseHttpError(resp));
        }

        await requested(getProject, 1).refreshAll();
        return attempt.ok<void>(void 0);
    }
);

function makeFetchProject(http: HttpClient) {
    return async (id: string) => {
        const fetched = await http.get(`projects/${id}`, {
            query: {
                fields: fields('Id,Name,Identifier,Summary,DescriptionJson,Tags,Version'),
            },
        });

        return fetched.pipe(
            attempt.flatMap(async (resp) => {
                if (!resp.ok) {
                    const err = await parseHttpError(resp);
                    return attempt.fail(err);
                }
                return await jsonify(() =>
                    resp.json<
                        Pick<
                            Project,
                            | 'id'
                            | 'name'
                            | 'identifier'
                            | 'summary'
                            | 'descriptionJson'
                            | 'tags'
                            | 'version'
                        > & { descriptionHtml: string | null }
                    >()
                );
            }),
            attempt.mapError(traced('fetch_project'))
        );
    };
}

export const getIsCreatedJustNow = query(type({ projectId: 'string' }), async (data) => {
    const e = getRequestEvent();
    const isCreatedJustNow = e.cookies.get('last_created_project') === data.projectId;
    if (isCreatedJustNow) {
        e.cookies.delete('last_created_project', {
            path: e.url.pathname,
            httpOnly: true,
            secure: true,
            sameSite: 'lax',
        });
    }
    return isCreatedJustNow;
});
