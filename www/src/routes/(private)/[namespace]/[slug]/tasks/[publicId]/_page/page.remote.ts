import { command, form, getRequestEvent, query } from '$app/server';
import { attempt } from '@duydang2311/attempt';
import { error, redirect } from '@sveltejs/kit';
import type { Activity } from '~/lib/models/activity';
import { cursorList, type CursorList } from '~/lib/models/paginated';
import type { User } from '~/lib/models/user';
import { BadHttpResponse, enrichStep, ValidationError } from '~/lib/utils/errors';
import { jsonify, parseHttpProblem } from '~/lib/utils/http';
import { useRuntime } from '~/lib/utils/runtime';
import { v } from '~/lib/utils/valibot';

export const addComment = command(
    v.object({
        taskId: v.string(),
        contentJson: v.record(v.string(), v.unknown()),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.post('comments', {
            body: data,
        });
        if (!result.ok) {
            return error(500, result.error);
        }

        const response = result.data;
        if (!response.ok) {
            if (response.status === 400) {
                return (await jsonify(() => response.json())).pipe(
                    attempt.mapError(enrichStep('parse_problem')),
                    attempt.flatMap(parseHttpProblem),
                    attempt.map(ValidationError.from),
                    attempt.flatMap(attempt.fail)
                );
            }
            return error(response.status, BadHttpResponse(response.status, await response.text()));
        }
        return attempt.ok({ success: true });
    }
);

export const deleteTask = form(
    v.object({
        id: v.pipe(v.string(), v.nonEmpty()),
    }),
    async (data) => {
        const e = getRequestEvent();
        const deleted = await e.locals.http.delete(`tasks/${data.id}`);
        if (!deleted.ok) {
            return deleted.error;
        }
        if (!deleted.data.ok) {
            return BadHttpResponse(deleted.data.status);
        }
        return redirect(303, `/${e.params.namespace}/${e.params.slug}/tasks`);
    }
);

export const editComment = command(
    v.object({
        id: v.string(),
        contentJson: v.string(),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = (
            await e.locals.http.patch(`comments/${data.id}`, {
                body: {
                    patch: {
                        contentJson: data.contentJson,
                    },
                },
            })
        ).pipe(
            attempt.flatMap(async (response) => {
                if (response.ok) {
                    return attempt.ok<void>(void 0);
                }
                return attempt.fail(BadHttpResponse(response.status, await response.text()));
            })
        );
        return result;
    }
);

export const deleteComment = command(
    v.object({
        id: v.string(),
    }),
    async (data) => {
        const e = getRequestEvent();
        return (await e.locals.http.delete(`comments/${data.id}`)).pipe(
            attempt.flatMap(async (response) => {
                if (response.ok) {
                    return attempt.ok<void>(void 0);
                }
                return attempt.fail(BadHttpResponse(response.status, await response.text()));
            })
        );
    }
);

export const getActivities = query(v.string(), async (taskId) => {
    const { http } = useRuntime();
    return (
        await http.get('activities', {
            query: {
                taskId,
                fields: 'Id,CreatedTime,Kind,Actor.Name,Actor.DisplayName,Actor.ImageKey,Actor.ImageVersion,Data',
                sort: 'Id',
                size: 20,
            },
        })
    ).pipe(
        attempt.flatMap((response) =>
            jsonify(() =>
                response.json<
                    CursorList<
                        Pick<Activity, 'id' | 'createdTime' | 'kind' | 'data'> & {
                            actor: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
                        },
                        string
                    >
                >()
            )
        ),
        attempt.unwrapOrElse(() => cursorList())
    );
});
