import { page } from '$app/state';
import { attempt } from '@duydang2311/attempt';
import { getContext, setContext } from 'svelte';
import { ActivityKind, type Activity } from '~/lib/models/activity';
import type { KeysetList } from '~/lib/models/paginated';
import type { User, UserPreset } from '~/lib/models/user';
import type { HttpClient } from '~/lib/services/http_client';
import { fields, jsonify, parseHttpError } from '~/lib/utils/http';
import { usePageData } from '~/lib/utils/kit';
import { v } from '~/lib/utils/valibot';
import { createValidator } from '~/lib/utils/validation';
import type { PageData } from '../$types';
import { getActivityList, getTask } from './page.remote';

export const validators = {
    [ActivityKind.Commented]: createValidator(
        v.object({
            comment: v.object({
                id: v.string(),
                contentJson: v.nullable(v.string()),
            }),
        })
    ),
    [ActivityKind.StatusChanged]: createValidator(
        v.object({
            status: v.nullable(v.object({ name: v.string() })),
            oldStatus: v.nullable(v.object({ name: v.string() })),
        })
    ),
    [ActivityKind.PriorityChanged]: createValidator(
        v.object({
            priority: v.nullable(v.object({ name: v.string() })),
            oldPriority: v.nullable(v.object({ name: v.string() })),
        })
    ),
    [ActivityKind.Assigned]: createValidator(
        v.object({
            assignee: v.object({
                id: v.string(),
                name: v.string(),
                displayName: v.nullable(v.string()),
            }),
        })
    ),
    [ActivityKind.Unassigned]: createValidator(
        v.object({
            assignee: v.object({
                id: v.string(),
                name: v.string(),
                displayName: v.nullable(v.string()),
            }),
        })
    ),
};

export interface ActivityListParams {
    taskId: string;
    size: number;
    afterId?: string | null;
}
export function makeFetchActivityList(http: HttpClient) {
    return async (params: ActivityListParams) => {
        const result = await http.get('activities', {
            query: {
                taskId: params.taskId,
                size: params.size,
                select: 'Id,CreatedTime,Kind,Actor.Id,Actor.Name,Actor.DisplayName,Actor.ImageKey,Actor.ImageVersion,Metadata',
                direction: 'asc',
                afterId: params.afterId,
            },
        });
        return result.pipe(
            attempt.flatMap(async (resp) => {
                if (!resp.ok) {
                    const err = await parseHttpError(resp);
                    return attempt.fail(err);
                }
                return await jsonify(() =>
                    resp.json<
                        KeysetList<
                            Pick<Activity, 'id' | 'createdTime' | 'kind' | 'metadata'> & {
                                actor: Pick<User, 'id'> & UserPreset['Avatar'];
                            }
                        >
                    >()
                );
            })
        );
    };
}

const key = {};

export function useTask() {
    const pageData = usePageData<PageData>();
    return getTask({
        projectId: pageData.project.id,
        taskPublicId: page.params.publicId!,
    });
}

export function useActivityLists(params: ActivityListParams[]) {
    return params.map((param) => ({
        param,
        query: getActivityList(param),
    }));
}

export function setPageContext(initial: { activityListParams: ActivityListParams[] }) {
    let activityListParams = $state(initial.activityListParams);
    return setContext(key, {
        get activityListParams() {
            return activityListParams;
        },
        set activityListParams(value) {
            activityListParams = value;
        },
    });
}

export function usePageContext() {
    return getContext(key) as ReturnType<typeof setPageContext>;
}

export function makeFetchTask(http: HttpClient) {
    return (data: { projectId: string; taskPublicId: string }) =>
        http.get(`projects/${data.projectId}/tasks/${data.taskPublicId}`, {
            query: {
                fields: fields(
                    'Id,PublicId,Title,CreatedTime,UpdatedTime,DescriptionJson,Version',
                    {
                        Status: 'Id,Name',
                        Priority: 'Id,Name',
                        Author: 'Name,DisplayName,ImageKey,ImageVersion',
                        Assignees: 'Id,Name,DisplayName,ImageKey,ImageVersion',
                    }
                ),
            },
        });
}
