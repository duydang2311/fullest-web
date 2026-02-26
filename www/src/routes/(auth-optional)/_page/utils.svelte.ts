import { attempt } from '@duydang2311/attempt';
import { getContext, setContext } from 'svelte';
import { ActivityKind, type Activity } from '~/lib/models/activity';
import type { Namespace } from '~/lib/models/namespace';
import { cursorList, keysetList, type CursorList, type KeysetList } from '~/lib/models/paginated';
import type { Project } from '~/lib/models/project';
import type { UserPreset } from '~/lib/models/user';
import type { HttpClient } from '~/lib/services/http_client';
import { BadHttpResponse } from '~/lib/utils/errors';
import { fields, jsonify } from '~/lib/utils/http';
import { v } from '~/lib/utils/valibot';
import { createValidator } from '~/lib/utils/validation';

export type LocalProject = Pick<Project, 'id' | 'name' | 'summary' | 'identifier'> & {
    namespace: Pick<Namespace, 'kind'> & { user: UserPreset['Avatar'] };
};

export type LocalActivity = Pick<
    Activity,
    'createdTime' | 'id' | 'kind' | 'projectId' | 'taskId' | 'metadata'
> & {
    actor: UserPreset['Avatar'];
};

export function getProjectList(http: HttpClient) {
    return async (
        memberId: string,
        afterId: string | null,
        untilId: string | null,
        direction: 'asc' | 'desc',
        size: number
    ) => {
        const result = await http.get('projects', {
            query: {
                memberId,
                select: fields('Id,Name,Summary,Identifier', {
                    Namespace: 'Kind,User.Name,User.DisplayName,User.ImageKey,User.ImageVersion',
                }),
                size,
                direction,
                afterId,
                untilId,
            },
        });
        return result.pipe(
            attempt.flatMap((resp) => jsonify(() => resp.json<KeysetList<LocalProject>>())),
            attempt.unwrapOrElse(keysetList)
        );
    };
}

export function getActivityList(http: HttpClient) {
    return async (userId: string) => {
        const result = await http.get('activities', {
            query: {
                forUserId: userId,
                select: fields('CreatedTime,Id,Kind,Metadata,ProjectId,TaskId', {
                    Actor: 'Id,Name,DisplayName,ImageKey,ImageVersion',
                    Project: fields('Id,Identifier,Name', { Namespace: 'Kind,User.Name' }),
                    Task: 'Id,PublicId,Title',
                }),
                sort: '-Id',
            },
        });
        return result.pipe(
            attempt.flatMap(async (resp) => {
                if (resp.ok) {
                    return await jsonify(() => resp.json<CursorList<LocalActivity, string>>());
                }
                console.error(resp);
                return attempt.fail(BadHttpResponse(resp.status));
            }),
            attempt.unwrapOrElse(() => cursorList())
        );
    };
}

const key = {};
export function setPageContext(initial: {
    projectList: KeysetList<LocalProject>;
    activityList?: CursorList<LocalActivity, string>;
}) {
    let projectList = $state.raw(initial?.projectList);
    let activityList = $state.raw(initial?.activityList);
    return setContext(key, {
        get projectList() {
            return projectList;
        },
        set projectList(value) {
            projectList = value;
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
    return getContext<ReturnType<typeof setPageContext>>(key);
}

export const activityValidators = {
    [ActivityKind.Created]: createValidator(
        v.object({
            actor: v.object({
                name: v.string(),
                displayName: v.nullable(v.string()),
            }),
        })
    ),
    [ActivityKind.Commented]: createValidator(
        v.object({
            actor: v.object({
                name: v.string(),
                displayName: v.nullable(v.string()),
            }),
            project: v.object({
                identifier: v.string(),
                namespace: v.object({
                    user: v.object({
                        name: v.string(),
                    }),
                }),
            }),
            task: v.object({
                title: v.string(),
                publicId: v.number(),
            }),
            metadata: v.object({
                comment: v.object({
                    id: v.string(),
                    contentJson: v.nullable(v.string()),
                }),
            }),
        })
    ),
    [ActivityKind.Assigned]: createValidator(
        v.object({
            actor: v.object({
                id: v.string(),
                name: v.string(),
                displayName: v.nullable(v.string()),
            }),
            metadata: v.object({
                assignee: v.object({
                    id: v.string(),
                    name: v.string(),
                    displayName: v.nullable(v.string()),
                }),
            }),
        })
    ),
    [ActivityKind.Unassigned]: createValidator(
        v.object({
            actor: v.object({
                id: v.string(),
                name: v.string(),
                displayName: v.nullable(v.string()),
            }),
            metadata: v.object({
                assignee: v.object({
                    id: v.string(),
                    name: v.string(),
                    displayName: v.nullable(v.string()),
                }),
            }),
        })
    ),
    [ActivityKind.StatusChanged]: createValidator(
        v.object({
            actor: v.object({
                id: v.string(),
                name: v.string(),
                displayName: v.nullable(v.string()),
            }),
            metadata: v.object({
                status: v.nullable(
                    v.object({
                        name: v.nullable(v.string()),
                    })
                ),
                oldStatus: v.nullable(
                    v.object({
                        name: v.nullable(v.string()),
                    })
                ),
            }),
        })
    ),
    [ActivityKind.PriorityChanged]: createValidator(
        v.object({
            actor: v.object({
                id: v.string(),
                name: v.string(),
                displayName: v.nullable(v.string()),
            }),
            metadata: v.object({
                priority: v.nullable(
                    v.object({
                        name: v.nullable(v.string()),
                    })
                ),
                oldPriority: v.nullable(
                    v.object({
                        name: v.nullable(v.string()),
                    })
                ),
            }),
        })
    ),
    [ActivityKind.TitleChanged]: createValidator(
        v.object({
            actor: v.object({
                name: v.string(),
                displayName: v.nullable(v.string()),
            }),
            metadata: v.object({
                title: v.string(),
                oldTitle: v.string(),
            }),
        })
    ),
    projectContext: createValidator(
        v.object({
            project: v.object({
                id: v.string(),
                name: v.string(),
                identifier: v.string(),
                namespace: v.object({
                    kind: v.string(),
                    user: v.nullish(
                        v.object({
                            name: v.string(),
                        })
                    ),
                }),
            }),
        })
    ),
    taskContext: createValidator(
        v.object({
            task: v.object({
                publicId: v.number(),
                title: v.string(),
            }),
        })
    ),
} as const;
