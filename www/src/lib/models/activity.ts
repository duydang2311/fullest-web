import type { Task } from './task';
import type { User } from './user';

export interface Activity {
    createdTime: string;
    id: string;
    taskId?: string;
    task?: Task;
    kind: ActivityKind;
    actorId: string;
    actor: User;
    data?: unknown;
}

export enum ActivityKind {
    Created = 'created',

    TitleUpdated = 'title_updated',

    Commented = 'commented',
    Assigned = 'assigned',
}
