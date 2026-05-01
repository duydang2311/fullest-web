import { command, getRequestEvent } from '$app/server';
import { attempt } from '@duydang2311/attempt';
import { type } from 'arktype';
import { parseHttpError } from '~/lib/utils/http';

export const editDescription = command(
    type({
        projectId: 'string',
        'descriptionJson?': 'string',
    }),
    async (data) => {
        const e = getRequestEvent();
        const result = await e.locals.http.patch(`projects/${data.projectId}`, {
            body: {
                patch: {
                    descriptionJson: data.descriptionJson || null,
                },
            },
        });
        if (!result.ok) {
            return attempt.fail(result.error);
        }

        const resp = result.data;
        if (!resp.ok) {
            return attempt.fail(await parseHttpError(resp));
        }

        return attempt.ok<void>(void 0);
    }
);
