import { attempt } from '@duydang2311/attempt';
import { createInfiniteQuery, createQuery } from '@tanstack/svelte-query';
import { getContext, setContext } from 'svelte';
import { ActivityKind, type Activity } from '~/lib/models/activity';
import type { CursorList } from '~/lib/models/paginated';
import type { User, UserPreset } from '~/lib/models/user';
import type { HttpClient } from '~/lib/services/http_client';
import { jsonify } from '~/lib/utils/http';
import { usePageData } from '~/lib/utils/kit';
import { QueryKey } from '~/lib/utils/query';
import { useRuntime } from '~/lib/utils/runtime';
import { v } from '~/lib/utils/valibot';
import { createValidator } from '~/lib/utils/validation';
import type { PageData } from '../$types';
import { getTask } from './page.remote';

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

export function fetchActivityList(http: HttpClient) {
    return async (taskId: string, after?: string | null, size?: number) => {
        const result = await http.get('activities', {
            query: {
                taskId,
                after,
                fields: 'Id,CreatedTime,Kind,Actor.Id,Actor.Name,Actor.DisplayName,Actor.ImageKey,Actor.ImageVersion,Data',
                size: size ?? 20,
            },
        });
        return result.pipe(
            attempt.flatMap((resp) =>
                jsonify(() =>
                    resp.json<
                        CursorList<
                            Pick<Activity, 'id' | 'createdTime' | 'kind' | 'data'> & {
                                actor: Pick<User, 'id'> & UserPreset['Avatar'];
                            },
                            string
                        >
                    >()
                )
            ),
            attempt.unwrap
        );
    };
}

const key = {};

export function setPageContext(options: {
    task: PageData['task'];
    activityList?: CursorList<
        Pick<Activity, 'id' | 'createdTime' | 'kind' | 'data'> & {
            actor: Pick<User, 'id'> & UserPreset['Avatar'];
        },
        string
    >;
}) {
    let task = $state.raw(options.task);
    let activityList = $state.raw<
        | CursorList<
              Pick<Activity, 'id' | 'createdTime' | 'kind' | 'data'> & {
                  actor: Pick<User, 'id'> & UserPreset['Avatar'];
              },
              string
          >
        | undefined
    >(options.activityList);
    return setContext(key, {
        get task() {
            return task;
        },
        set task(value) {
            task = value;
        },
        get activityList() {
            return activityList;
        },
        set activityList(value) {
            activityList = value;
        },
    });
}

export function usePageContext() {
    return getContext(key) as ReturnType<typeof setPageContext>;
}

export function invalidateActivityList(ctx: ReturnType<typeof setPageContext>, http: HttpClient) {
    return ()  => {};
    return async () => {
        ctx.activityList = await fetchActivityList(http)(
            ctx.task.id,
            null,
            ctx.activityList?.items.length
        );
    };
}
