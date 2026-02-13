import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import type { Comment } from '~/lib/models/comment';
import type { Priority } from '~/lib/models/priority';
import type { Status } from '~/lib/models/status';
import type { Task } from '~/lib/models/task';
import type { User, UserPreset } from '~/lib/models/user';
import { ErrorKind, ForbiddenError, NotFoundError, UnknownError } from '~/lib/utils/errors';
import { jsonify } from '~/lib/utils/http';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
    e.depends(e.route.id);
    const parent = await e.parent();
    const fetchedTask = await e.locals.http.get(
        `projects/${parent.project.id}/tasks/${e.params.publicId}`,
        {
            query: {
                fields: [
                    'Id,PublicId,Title,Status.Id,Status.Name,Priority.Id,Priority.Name,CreatedTime,UpdatedTime,InitialCommentId,Author.Name',
                    'InitialComment.Id,InitialComment.ContentJson,InitialComment.CreatedTime,InitialComment.Author.Name',
                    'InitialComment.Author.DisplayName,InitialComment.Author.ImageKey,InitialComment.Author.ImageVersion',
                    'Assignees.Id,Assignees.Name,Assignees.DisplayName,Assignees.ImageKey,Assignees.ImageVersion',
                ].join(','),
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
                        | 'initialCommentId'
                    > & {
                        priority: Pick<Priority, 'id' | 'name'>;
                        status: Pick<Status, 'id' | 'name'>;
                        initialComment: Pick<Comment, 'id' | 'contentJson' | 'createdTime'> & {
                            author: Pick<
                                User,
                                'name' | 'displayName' | 'imageKey' | 'imageVersion'
                            >;
                        };
                        author: Pick<User, 'name'>;
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

    return {
        task,
    };
};
