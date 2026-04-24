import { error } from '@sveltejs/kit';
import { reverseKeysetList } from '~/lib/models/paginated';
import { Err } from '~/lib/utils/errors';
import { observableServerLoad } from '~/lib/utils/observability';
import type { PageServerLoad } from './$types';
import { getProjectList } from './_page/utils.svelte';

export const load = observableServerLoad<PageServerLoad>(async (e) => {
    error(500, Err('ERR_MISSING_OAUTH_STATE'));
    const session = e.locals.session;
    if (session) {
        const lastId = e.untrack(() => e.url.searchParams.get('p'));
        const projectList = await getProjectList(e.locals.http)(
            session.user.id,
            null,
            lastId,
            lastId ? 'desc' : 'asc',
            lastId ? 20 : 5
        );
        return {
            session,
            projectList: lastId ? reverseKeysetList(projectList) : projectList,
        };
    }
    return {
        session,
    };
});
