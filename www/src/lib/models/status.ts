import type { Project } from './project';

export interface Status {
    id: string;
    name: string;
    category: StatusCategory;
    color: string;
    rank: string;
    description?: string;
    projectId: string;
    project: Project;
}

export enum StatusCategory {
    Proposed = 'proposed',
    Ready = 'ready',
    Active = 'active',
    Paused = 'paused',
    Review = 'review',
    Completed = 'completed',
    Canceled = 'canceled',
    Archived = 'archived',
}
