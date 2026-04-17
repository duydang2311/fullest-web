import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import sanitize from 'sanitize-html';
import { renderToHTMLString } from '~/lib/components/editor';
import type { Priority } from '~/lib/models/priority';
import type { Status } from '~/lib/models/status';
import type { Task } from '~/lib/models/task';
import type { User, UserPreset } from '~/lib/models/user';
import { ErrorKind, ForbiddenError, NotFoundError, UnknownError } from '~/lib/utils/errors';
import { fields, jsonify } from '~/lib/utils/http';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
    e.depends(e.route.id);
    const parent = await e.parent();
    const fetchedTask = await e.locals.http.get(
        `projects/${parent.project.id}/tasks/${e.params.publicId}`,
        {
            query: {
                fields: fields('Id,PublicId,Title,CreatedTime,UpdatedTime,DescriptionJson', {
                    Status: 'Id,Name',
                    Priority: 'Id,Name',
                    Author: 'Name,DisplayName,ImageKey,ImageVersion',
                    Assignees: 'Id,Name,DisplayName,ImageKey,ImageVersion',
                }),
            },
        }
    );

    const task = await fetchedTask.pipe(
        attempt.flatMap(async (response) => {
            if (response.status === 403) {
                return attempt.fail(ForbiddenError());
            }
            if (response.status === 404) {
                return attempt.fail(NotFoundError());
            }
            if (!response.ok) {
                return attempt.fail(UnknownError(await response.text()));
            }
            return jsonify(() =>
                response.json<
                    Pick<
                        Task,
                        | 'id'
                        | 'publicId'
                        | 'title'
                        | 'createdTime'
                        | 'updatedTime'
                        | 'descriptionJson'
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
        attempt.unwrapOrElse((e) => {
            return error(
                e.kind === ErrorKind.Forbidden ? 403 : e.kind === ErrorKind.NotFound ? 404 : 500,
                e
            );
        })
    );

    console.log(task);
    if (task.descriptionJson) {
        task.descriptionHtml = sanitize(renderToHTMLString(JSON.parse(task.descriptionJson)));
    }

    return {
        task,
    };
};
