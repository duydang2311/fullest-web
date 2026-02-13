import { QueryKey, useQueryClient } from '~/lib/utils/query';
import type { PageLoad } from './$types';

export const load: PageLoad = async (e) => {
    const queryClient = useQueryClient();
    queryClient.setQueryData(
        QueryKey.TaskDetailsPage.ofTask({ taskId: e.data.task.id }),
        e.data.task
    );
    return e.data;
};
