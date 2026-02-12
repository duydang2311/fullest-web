import { ActivityKind } from '~/lib/models/activity';
import { v } from '~/lib/utils/valibot';
import { createValidator } from '~/lib/utils/validation';

export const validators = {
    [ActivityKind.Commented]: createValidator(
        v.object({
            comment: v.object({
                id: v.string(),
                contentJson: v.nullish(v.string()),
            }),
        })
    ),
    [ActivityKind.StatusChanged]: createValidator(
        v.object({
            status: v.nullish(v.object({ name: v.string() })),
            oldStatus: v.nullish(v.object({ name: v.string() })),
        })
    ),
    [ActivityKind.PriorityChanged]: createValidator(
        v.object({
            priority: v.nullish(v.object({ name: v.string() })),
            oldPriority: v.nullish(v.object({ name: v.string() })),
        })
    ),
    [ActivityKind.Assigned]: createValidator(
        v.object({
            assignee: v.object({
                id: v.string(),
                name: v.string(),
                displayName: v.nullish(v.string()),
            }),
        })
    ),
    [ActivityKind.Unassigned]: createValidator(
        v.object({
            assignee: v.object({
                id: v.string(),
                name: v.string(),
                displayName: v.nullish(v.string()),
            }),
        })
    ),
};
