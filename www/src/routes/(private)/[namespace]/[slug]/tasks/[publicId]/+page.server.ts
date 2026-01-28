import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import type { Comment } from '~/lib/models/comment';
import { keysetList, type KeysetList } from '~/lib/models/paginated';
import type { Task } from '~/lib/models/task';
import type { User } from '~/lib/models/user';
import { ErrorKind, ForbiddenError, NotFoundError, UnknownError } from '~/lib/utils/errors';
import { jsonify } from '~/lib/utils/http';
import { useRuntime } from '~/lib/utils/runtime';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
    e.depends(e.route.id);
    const parent = await e.parent();
    const fetchedTask = await e.locals.http.get(
        `projects/${parent.project.id}/tasks/${e.params.publicId}`,
        {
            query: {
                fields: [
                    'Id,PublicId,Title,Status,Priority,CreatedTime,UpdatedTime,InitialCommentId,Author.Name',
                    'InitialComment.Id,InitialComment.ContentJson,InitialComment.CreatedTime,InitialComment.Author.Name',
                    'InitialComment.Author.DisplayName,InitialComment.Author.ImageKey,InitialComment.Author.ImageVersion',
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
                        | 'status'
                        | 'priority'
                        | 'createdTime'
                        | 'updatedTime'
                        | 'initialCommentId'
                    > & {
                        initialComment: Pick<Comment, 'id' | 'contentJson' | 'createdTime'> & {
                            author: Pick<
                                User,
                                'name' | 'displayName' | 'imageKey' | 'imageVersion'
                            >;
                        };
                        author: Pick<User, 'name'>;
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
        streamedCommentList: fetchComments(task.id).then((fetched) =>
            fetched.pipe(attempt.unwrapOrElse(() => keysetList<never, never>()))
        ),
        ts: Date.now()
    };
};

async function fetchComments(taskId: string) {
    const { http } = useRuntime();
    return (
        await http.get('comments/keyset', {
            query: {
                taskId,
                fields: 'Id,ContentJson,CreatedTime,Author.Name,Author.DisplayName,Author.ImageKey,Author.ImageVersion',
                sort: 'Id',
                size: 20,
            },
        })
    ).pipe(
        attempt.flatMap((response) =>
            jsonify(() =>
                response.json<
                    KeysetList<
                        Pick<Comment, 'id' | 'contentJson' | 'createdTime'> & {
                            author: Pick<
                                User,
                                'name' | 'displayName' | 'imageKey' | 'imageVersion'
                            >;
                        },
                        string
                    >
                >()
            )
        )
    );
}
