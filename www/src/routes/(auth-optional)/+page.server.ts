import { reverseKeysetList } from '~/lib/models/paginated';
import type { PageServerLoad } from './$types';
import { getProjectList } from './_page/utils.svelte';

export const load: PageServerLoad = async (e) => {
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
};
