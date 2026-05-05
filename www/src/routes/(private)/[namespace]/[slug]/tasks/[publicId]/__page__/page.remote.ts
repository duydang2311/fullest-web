import { command, form, getRequestEvent, query, requested } from '$app/server';
import { attempt } from '@duydang2311/attempt';
import { error, redirect } from '@sveltejs/kit';
import { type } from 'arktype';
import sanitize from 'sanitize-html';
import { renderToHTMLString } from '~/lib/components/editor.svelte';
import { cursorList, offsetList, type CursorList, type OffsetList } from '~/lib/models/paginated';
import type { Priority } from '~/lib/models/priority';
import type { Status } from '~/lib/models/status';
import type { Task } from '~/lib/models/task';
import type { User, UserPreset } from '~/lib/models/user';
import { BadHttpResponse, enrichStep, Err, ValidationError } from '~/lib/utils/errors';
import { jsonify, parseHttpError, parseHttpProblem } from '~/lib/utils/http';
import { v } from '~/lib/utils/valibot';
import { makeFetchActivityList, makeFetchTask } from './utils.svelte';

export const addComment = command(
    v.object({
        taskId: v.string(),
        contentJson: v.string(),
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
        await requested(getActivityList, Infinity).refreshAll();
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
    type({
        id: 'string',
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.delete(`comments/${data.id}`);
        if (!result.ok) {
            return result;
        }

        const resp = result.data;
        if (!resp.ok) {
            const error = await parseHttpError(resp);
            return attempt.fail(error);
        }

        await requested(getActivityList, Infinity).refreshAll();
        return attempt.ok<void>(void 0);
    }
);

export const getTask = query(
    v.object({
        projectId: v.string(),
        taskPublicId: v.string(),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await makeFetchTask(e.locals.http)(data);
        return result.pipe(
            attempt.flatMap(async (resp) => {
                if (!resp.ok) {
                    const err = await parseHttpError(resp);
                    return error(err.status, err);
                }
                return await jsonify(() =>
                    resp.json<
                        Pick<
                            Task,
                            | 'id'
                            | 'publicId'
                            | 'title'
                            | 'createdTime'
                            | 'updatedTime'
                            | 'descriptionJson'
                            | 'version'
                        > & {
                            descriptionHtml: string;
                            priority: Pick<Priority, 'id' | 'name'>;
                            status: Pick<Status, 'id' | 'name'>;
                            author: UserPreset['Avatar'];
                            assignees: (Pick<User, 'id'> & UserPreset['Avatar'])[];
                        }
                    >()
                );
            }),
            attempt.map((task) => {
                if (task.descriptionJson) {
                    task.descriptionHtml = sanitize(
                        renderToHTMLString(JSON.parse(task.descriptionJson))
                    );
                }
                return task;
            }),
            attempt.unwrapOrElse((e) => error(500, e))
        );
    }
);

export const getActivityList = query(
    v.object({
        taskId: v.string(),
        size: v.number(),
        afterId: v.nullish(v.string()),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await makeFetchActivityList(e.locals.http)(data);
        return result.pipe(attempt.unwrapOrElse((e) => error(500, e)));
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
        version: v.number(),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.patch(`tasks/${data.taskId}`, {
            body: {
                version: data.version,
                patch: {
                    statusId: data.statusId,
                },
            },
        });
        if (!result.ok) {
            return result;
        }

        const resp = result.data;
        if (!resp.ok) {
            const error = await parseHttpError(resp);
            return attempt.fail(error);
        }

        const body = await jsonify(() => resp.json<{ version: number }>());
        if (!body.ok) {
            return attempt.fail(body.error);
        }

        await Promise.all([
            requested(getActivityList, Infinity).refreshAll(),
            requested(getTask, 1).refreshAll(),
        ]);
        return attempt.ok(body.data);
    }
);

export const getPriorities = query(v.string(), async (projectId) => {
    const e = getRequestEvent();
    return (
        await e.locals.http.get('priorities', {
            query: {
                projectId,
                fields: 'Id,Name,Color,Rank,Category',
                sort: 'Rank',
                size: 20,
            },
        })
    ).pipe(
        attempt.flatMap((response) =>
            jsonify(() =>
                response.json<
                    OffsetList<Pick<Priority, 'id' | 'name' | 'color' | 'rank' | 'category'>>
                >()
            )
        ),
        attempt.unwrapOrElse(() => offsetList())
    );
});

export const patchTaskPriority = command(
    v.object({
        taskId: v.string(),
        priorityId: v.string(),
        version: v.number(),
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.patch(`tasks/${data.taskId}`, {
            body: {
                version: data.version,
                patch: {
                    priorityId: data.priorityId,
                },
            },
        });
        if (!result.ok) {
            return result;
        }

        const resp = result.data;
        if (!resp.ok) {
            const error = await parseHttpError(resp);
            return attempt.fail(error);
        }

        const body = await jsonify(() => resp.json<{ version: number }>());
        if (!body.ok) {
            return attempt.fail(body.error);
        }

        await Promise.all([
            requested(getActivityList, Infinity).refreshAll(),
            requested(getTask, 1).refreshAll(),
        ]);
        return attempt.ok(body.data);
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

        const resp = result.data;
        if (!resp.ok) {
            const error = await parseHttpError(resp);
            return attempt.fail(error);
        }
        await requested(getTask, 1).refreshAll();
        return attempt.ok<void>(void 0);
    }
);

export const updateTaskAssignees = command(
    type({
        taskId: 'string > 0',
        assigned: 'string[]',
        unassigned: 'string[]',
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.post('task-assignees/batch', {
            body: {
                taskId: data.taskId,
                assigned: data.assigned,
                unassigned: data.unassigned,
            },
        });
        if (!result.ok) {
            return result;
        }

        const resp = result.data;
        if (!resp.ok) {
            const error = await parseHttpError(resp);
            return attempt.fail(error);
        }
        await Promise.all([
            requested(getActivityList, Infinity).refreshAll(),
            requested(getTask, 1).refreshAll(),
        ]);
        return attempt.ok<void>(void 0);
    }
);

export const editTaskTitle = form(
    type({
        taskId: 'string > 0',
        title: 'string > 0',
        version: 'number.integer',
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.patch(`tasks/${data.taskId}`, {
            body: {
                version: data.version,
                patch: {
                    title: data.title,
                },
            },
        });
        if (!result.ok) {
            return result;
        }

        const resp = result.data;
        if (!resp.ok) {
            const error = await parseHttpError(resp);
            return attempt.fail(error);
        }

        await Promise.all([
            requested(getTask, 1).refreshAll(),
            requested(getActivityList, Infinity).refreshAll(),
        ]);
        return attempt.ok<void>(void 0);
    }
);

export const editTaskDescription = command(
    type({
        taskId: 'string > 0',
        descriptionJson: 'string > 0',
        version: 'number.integer',
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.patch(`tasks/${data.taskId}`, {
            body: {
                version: data.version,
                patch: {
                    descriptionJson: data.descriptionJson,
                },
            },
        });
        if (!result.ok) {
            return result;
        }

        const resp = result.data;
        if (!resp.ok) {
            const error = await parseHttpError(resp);
            return attempt.fail(error);
        }

        await requested(getTask, 1).refreshAll();
        return attempt.ok<void>(void 0);
    }
);
