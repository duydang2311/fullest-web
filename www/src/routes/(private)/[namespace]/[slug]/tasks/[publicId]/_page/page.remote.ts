import { command, form, getRequestEvent, query } from '$app/server';
import { attempt } from '@duydang2311/attempt';
import { error, redirect } from '@sveltejs/kit';
import type { Activity } from '~/lib/models/activity';
import { cursorList, offsetList, type CursorList, type OffsetList } from '~/lib/models/paginated';
import type { Priority } from '~/lib/models/priority';
import type { Status } from '~/lib/models/status';
import type { User, UserPreset } from '~/lib/models/user';
import { BadHttpResponse, enrichStep, ValidationError } from '~/lib/utils/errors';
import { jsonify, parseHttpProblem } from '~/lib/utils/http';
import { v } from '~/lib/utils/valibot';
import type { PageData } from '../$types';

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

export const getTask = query(
    v.object({
        taskId: v.string(),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.get(`tasks/${data.taskId}`, {
            query: {
                fields: [
                    'Id,PublicId,Title,Status.Id,Status.Name,Priority.Id,Priority.Name,CreatedTime,UpdatedTime,InitialCommentId,Author.Name',
                    'InitialComment.Id,InitialComment.ContentJson,InitialComment.CreatedTime,InitialComment.Author.Name',
                    'InitialComment.Author.DisplayName,InitialComment.Author.ImageKey,InitialComment.Author.ImageVersion',
                    'Assignees.Id,Assignees.Name,Assignees.DisplayName,Assignees.ImageKey,Assignees.ImageVersion',
                ].join(','),
            },
        });
        return await result.pipe(
            attempt.flatMap((response) => jsonify(() => response.json<PageData['task']>()))
        );
    }
);

export const getActivities = query(
    v.object({
        taskId: v.string(),
        size: v.nullish(v.number()),
        cursor: v.nullish(v.string()),
    }),
    async (data) => {
        const e = getRequestEvent();
        return (
            await e.locals.http.get('activities', {
                query: {
                    taskId: data.taskId,
                    cursor: data.cursor,
                    fields: 'Id,CreatedTime,Kind,Actor.Id,Actor.Name,Actor.DisplayName,Actor.ImageKey,Actor.ImageVersion,Data',
                    sort: 'Id',
                    size: data.size ?? 20,
                },
            })
        ).pipe(
            attempt.flatMap((response) =>
                jsonify(() =>
                    response.json<
                        CursorList<
                            Pick<Activity, 'id' | 'createdTime' | 'kind' | 'data'> & {
                                actor: Pick<User, 'id'> & UserPreset['Avatar'];
                            },
                            string
                        >
                    >()
                )
            ),
            attempt.unwrapOrElse(() => cursorList())
        );
    }
);

export const getStatuses = query(v.string(), async (projectId) => {
    const e = getRequestEvent();
    return (
        await e.locals.http.get('statuses', {
            query: {
                projectId,
                fields: 'Id,Name,Category,Color,Rank',
                sort: 'Rank',
                size: 20,
            },
        })
    ).pipe(
        attempt.flatMap((response) =>
            jsonify(() =>
                response.json<
                    OffsetList<Pick<Status, 'id' | 'name' | 'category' | 'color' | 'rank'>>
                >()
            )
        ),
        attempt.unwrapOrElse(() => offsetList())
    );
});

export const patchTaskStatus = command(
    v.object({
        taskId: v.string(),
        statusId: v.string(),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.patch(`tasks/${data.taskId}`, {
            body: {
                patch: {
                    statusId: data.statusId,
                },
            },
        });
        if (!result.ok) {
            return result;
        }

        const response = result.data;
        if (!response.ok) {
            return attempt.fail(BadHttpResponse(response.status, response.statusText));
        }

        return attempt.ok<void>(void 0);
    }
);

export const getPriorities = query(v.string(), async (projectId) => {
    const e = getRequestEvent();
    return (
        await e.locals.http.get('priorities', {
            query: {
                projectId,
                fields: 'Id,Name,Color,Rank',
                sort: 'Rank',
                size: 20,
            },
        })
    ).pipe(
        attempt.flatMap((response) =>
            jsonify(() =>
                response.json<OffsetList<Pick<Priority, 'id' | 'name' | 'color' | 'rank'>>>()
            )
        ),
        attempt.unwrapOrElse(() => offsetList())
    );
});

export const patchTaskPriority = command(
    v.object({
        taskId: v.string(),
        priorityId: v.string(),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.patch(`tasks/${data.taskId}`, {
            body: {
                patch: {
                    priorityId: data.priorityId,
                },
            },
        });
        if (!result.ok) {
            return result;
        }

        const response = result.data;
        if (!response.ok) {
            return attempt.fail(BadHttpResponse(response.status, response.statusText));
        }

        return attempt.ok<void>(void 0);
    }
);

export const searchUsers = query(
    v.object({
        query: v.string(),
    }),
    async (data) => {
        const e = getRequestEvent();
        return (
            await e.locals.http.get(`users`, {
                query: {
                    search: data.query,
                    fields: 'Id,Name,DisplayName,ImageKey,ImageVersion',
                },
            })
        ).pipe(
            attempt.flatMap((a) =>
                jsonify(() => a.json<CursorList<Pick<User, 'id'> & UserPreset['Avatar'], string>>())
            ),
            attempt.unwrapOrElse((e) => {
                console.error(e);
                return cursorList();
            })
        );
    }
);

export const assignTask = command(
    v.object({
        taskId: v.string(),
        userId: v.string(),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.post('task-assignees', {
            body: {
                taskId: data.taskId,
                userId: data.userId,
            },
        });
        if (!result.ok) {
            return result;
        }

        const response = result.data;
        if (!response.ok) {
            return attempt.fail(BadHttpResponse(response.status, response.statusText));
        }

        return attempt.ok<void>(void 0);
    }
);

export const unassignTask = command(
    v.object({
        taskId: v.string(),
        userId: v.string(),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.delete(`tasks/${data.taskId}/assignees/${data.userId}`);
        if (!result.ok) {
            return result;
        }

        const response = result.data;
        if (!response.ok) {
            return attempt.fail(BadHttpResponse(response.status, response.statusText));
        }

        return attempt.ok<void>(void 0);
    }
);
