import type { User } from './user';

export interface Activity {
    createdTime: string;
    id: string;
    kind: ActivityKind;
    actorId: string;
    actor: User;
    projectId: string | null;
    taskId: string | null;
    metadata: string | null;
}

export enum ActivityKind {
    Created = 'created',

    Commented = 'commented',
    Assigned = 'assigned',
    Unassigned = 'unassigned',
    StatusChanged = 'status_changed',
    PriorityChanged = 'priority_changed',
    TitleChanged = 'title_changed',
}
