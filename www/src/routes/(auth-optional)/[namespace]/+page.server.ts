import { attempt } from '@duydang2311/attempt';
import { error } from '@sveltejs/kit';
import type { Namespace } from '~/lib/models/namespace';
import type { User } from '~/lib/models/user';
import { BadHttpResponse, ErrorKind, NotFoundError } from '~/lib/utils/errors';
import { jsonify } from '~/lib/utils/http';
import { useRuntime } from '~/lib/utils/runtime';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async (e) => {
    const fetchedNs = await fetchNamespace(e.params.namespace);
    if (!fetchedNs.ok) {
        if (fetchedNs.error.kind === ErrorKind.NotFound) {
            return error(404, fetchedNs.error);
        }
        if (fetchedNs.error.kind === ErrorKind.BadHttpResponse) {
            return error(fetchedNs.error.status, fetchedNs.error);
        }
        return error(500, fetchedNs.error);
    }
    return {
        session: e.locals.session,
        namespace: fetchedNs.data,
    };
};

async function fetchNamespace(slug: string) {
    const { http } = useRuntime();
    return (
        await http.get(`namespaces/${slug}`, {
            query: {
                fields: 'Kind,User.Id,User.Name,User.DisplayName,User.ImageKey,User.ImageVersion',
            },
        })
    ).pipe(
        attempt.flatMap(async (response) => {
            if (response.ok) {
                return jsonify(() =>
                    response.json<
                        Pick<Namespace, 'kind'> & {
                            user: Pick<
                                User,
                                'id' | 'name' | 'displayName' | 'imageKey' | 'imageVersion'
                            >;
                        }
                    >()
                );
            }
            switch (response.status) {
                case 404:
                    return attempt.fail(NotFoundError());
                default:
                    return attempt.fail(BadHttpResponse(response.status));
            }
        })
    );
}
