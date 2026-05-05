import { attempt } from '@duydang2311/attempt';
import { getContext, setContext } from 'svelte';
import { keysetList, type KeysetList } from '~/lib/models/paginated';
import type { Priority } from '~/lib/models/priority';
import type { Status } from '~/lib/models/status';
import type { Task } from '~/lib/models/task';
import type { UserPreset } from '~/lib/models/user';
import type { HttpClient } from '~/lib/services/http_client';
import { fields, jsonify, type Direction } from '~/lib/utils/http';

export type LocalTask = Pick<Task, 'id' | 'publicId' | 'title'> & {
    author: UserPreset['Avatar'];
    assignees?: UserPreset['Avatar'][];
    status?: Pick<Status, 'id'>;
    priority?: Pick<Priority, 'category' | 'name' | 'color'>;
};

export function selectLocalTask() {
    return fields('Id,PublicId,Title,Status.Id', {
        Author: 'Name,DisplayName,ImageKey,ImageVersion',
        Assignees: 'Name,DisplayName,ImageKey,ImageVersion',
        Priority: 'Category,Name,Color',
    });
}

export function makeFetchTaskList(http: HttpClient) {
    return async (query: {
        projectId: string;
        hasStatusFilter: boolean;
        direction: Direction;
        size: number;
        afterId?: string | null;
        untilId?: string | null;
        statusId?: string | null;
        includeTotalCount?: boolean;
    }) => {
        const result = await http.get('tasks', {
            query: {
                ...query,
                select: selectLocalTask(),
            },
        });
        return result.pipe(
            attempt.flatMap(async (resp) =>
                resp.ok
                    ? await jsonify(() => resp.json<KeysetList<LocalTask>>())
                    : attempt.fail<void>(void 0)
            ),
            attempt.unwrapOrElse(keysetList)
        );
    };
}

const key = {};
export function setPageContext(initial: { taskGroups: Record<string, KeysetList<LocalTask>> }) {
    let taskGroups = $state(initial.taskGroups);
    return setContext(key, {
        get taskGroups() {
            return taskGroups;
        },
        set taskGroups(value) {
            taskGroups = value;
        },
    });
}

export function usePageContext() {
    return getContext(key) as ReturnType<typeof setPageContext>;
}
