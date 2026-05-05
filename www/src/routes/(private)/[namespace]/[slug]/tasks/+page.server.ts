import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import type { KeysetList, OffsetList } from '~/lib/models/paginated';
import type { Status } from '~/lib/models/status';
import { enrichStep, traced } from '~/lib/utils/errors';
import { jsonify, parseHttpError } from '~/lib/utils/http';
import type { PageServerLoad } from './$types';
import { selectLocalTask, type LocalTask } from './__page__/utils.svelte';

export const load: PageServerLoad = async (e) => {
    const parent = await e.parent();
    const [statusList, fetchedTasks] = await Promise.all([
        e.locals.http
            .get('statuses', {
                query: {
                    projectId: parent.project.id,
                    fields: 'Id,Name,Color,Rank,Category',
                    sort: 'Rank',
                },
            })
            .then((att) =>
                att.pipe(
                    attempt.flatMap((response) =>
                        jsonify(() =>
                            response.json<
                                OffsetList<
                                    Pick<Status, 'id' | 'name' | 'color' | 'rank' | 'category'>
                                >
                            >()
                        )
                    ),
                    attempt.unwrapOrElse((e) => error(500, enrichStep('fetch_statuses')(e)))
                )
            ),
        e.locals.http
            .get('tasks/grouped/status', {
                query: {
                    projectId: parent.project.id,
                    select: selectLocalTask(),
                    size: 10,
                    includeTotalCount: true,
                    direction: 'desc',
                },
            })
            .then((att) =>
                att.pipe(
                    attempt.flatMap(async (response) =>
                        response.ok
                            ? await jsonify(() =>
                                  response.json<Record<string, KeysetList<LocalTask>>>()
                              )
                            : attempt.fail(await parseHttpError(response))
                    )
                )
            ),
    ]);

    if (!fetchedTasks.ok) {
        return error(
            fetchedTasks.error.kind === 'HttpError' ||
                fetchedTasks.error.kind === 'HttpValidationError'
                ? fetchedTasks.error.status
                : 500,
            traced('fetch_tasks')(fetchedTasks.error)
        );
    }

    return {
        statusList,
        taskList: fetchedTasks.data,
    };
};
