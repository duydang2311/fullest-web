import { form, getRequestEvent } from '$app/server';
import { attempt } from '@duydang2311/attempt';
import { error, fail, redirect } from '@sveltejs/kit';
import { type } from 'arktype';
import type { Task } from '~/lib/models/task';
import { Err, traced } from '~/lib/utils/errors';
import { jsonify, parseHttpError } from '~/lib/utils/http';

export const createTask = form(
    type({
        projectId: 'string',
        title: 'string',
        'descriptionJson?': 'string',
    }),
    async (data) => {
        console.log('data', data);
        const e = getRequestEvent();
        const result = await e.locals.http.post('tasks', {
            body: {
                projectId: data.projectId,
                title: data.title,
                descriptionJson: data.descriptionJson || null,
            },
        });
        if (!result.ok) {
            return error(500, result.error);
        }

        const resp = result.data;
        if (!resp.ok) {
            const err = await parseHttpError(resp);
            return fail(err.status, err);
        }

        const task = await jsonify(() => resp.json<Pick<Task, 'id' | 'publicId'>>()).then(
            attempt.unwrapOrElse((e) => error(500, traced('jsonify_response')(e)))
        );

        const path = `/${e.params.namespace}/${e.params.slug}/tasks/${task.publicId}`;
        e.cookies.set('last_created_task_id', task.id, {
            path,
            secure: true,
            httpOnly: true,
            sameSite: 'lax',
        });
        return redirect(302, path);
    }
);
