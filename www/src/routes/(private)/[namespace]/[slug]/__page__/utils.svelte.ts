import { usePageData } from '~/lib/utils/kit';
import { getIsCreatedJustNow, getProject } from './page.remote';
import type { PageData } from '../$types';

export function useProject() {
    const pageData = usePageData<PageData>();
    return getProject({ id: pageData.project.id });
}

export function useIsCreatedJustNow() {
    const pageData = usePageData<PageData>();
    return getIsCreatedJustNow({ projectId: pageData.project.id });
}
