import type { Priority } from '~/lib/models/priority';
import type { Status } from '~/lib/models/status';
import type { Task } from '~/lib/models/task';
import type { UserPreset } from '~/lib/models/user';

export type LocalTask = Pick<Task, 'id' | 'publicId' | 'title'> & {
    author: UserPreset['Avatar'];
    status?: Pick<Status, 'id'>;
    priority?: Pick<Priority, 'name'>;
};
