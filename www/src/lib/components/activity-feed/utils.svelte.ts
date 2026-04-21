import { type } from 'arktype';
import type { Component } from 'svelte';
import { ActivityKind } from '~/lib/models/activity';
import { createValidator, type Validator } from '~/lib/utils/validation';

export interface ActivityRender<T> {
    validator: Validator<T>;
    component: Component<{ activity: T }>;
}

export const activityValidators = {
    [ActivityKind.Created]: createValidator(
        type({
            actor: {
                name: 'string',
                displayName: 'string | null',
            },
        })
    ),
    [ActivityKind.Commented]: createValidator(
        type({
            actor: {
                name: 'string',
                displayName: 'string | null',
            },
            metadata: {
                comment: {
                    id: 'string',
                    contentJson: 'string | null',
                },
            },
        })
    ),
    [ActivityKind.Assigned]: createValidator(
        type({
            actor: {
                id: 'string',
                name: 'string',
                displayName: 'string | null',
            },
            metadata: {
                assignee: {
                    id: 'string',
                    name: 'string',
                    displayName: 'string | null',
                },
            },
        })
    ),
    [ActivityKind.Unassigned]: createValidator(
        type({
            actor: {
                id: 'string',
                name: 'string',
                displayName: 'string | null',
            },
            metadata: {
                assignee: {
                    id: 'string',
                    name: 'string',
                    displayName: 'string | null',
                },
            },
        })
    ),
    [ActivityKind.StatusChanged]: createValidator(
        type({
            actor: {
                id: 'string',
                name: 'string',
                displayName: 'string | null',
            },
            metadata: {
                status: type({
                    name: 'string | null',
                }).or(type.null),
                oldStatus: type({
                    name: 'string | null',
                }).or(type.null),
            },
        })
    ),
    [ActivityKind.PriorityChanged]: createValidator(
        type({
            actor: {
                id: 'string',
                name: 'string',
                displayName: 'string | null',
            },
            metadata: {
                priority: type({
                    name: 'string | null',
                }).or(type.null),
                oldPriority: type({
                    name: 'string | null',
                }).or(type.null),
            },
        })
    ),
    [ActivityKind.TitleChanged]: createValidator(
        type({
            actor: {
                name: 'string',
                displayName: 'string | null',
            },
            metadata: {
                title: 'string',
                oldTitle: 'string',
            },
        })
    ),
    projectContext: createValidator(
        type({
            project: {
                id: 'string',
                name: 'string',
                identifier: 'string',
                namespace: {
                    kind: 'string',
                    'user?': type({
                        name: 'string',
                    }).or(type.null),
                },
            },
        })
    ),
    taskContext: createValidator(
        type({
            task: {
                publicId: 'number',
                title: 'string',
            },
        })
    ),
} as const;
