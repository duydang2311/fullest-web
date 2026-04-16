import type { Project } from './project';

export interface Priority {
    id: string;
    name: string;
    color: string;
    rank: string;
    category: PriorityCategory;
    description?: string;
    projectId: string;
    project: Project;
}

export enum PriorityCategory {
    Low = 'low',
    Medium = 'medium',
    High = 'high',
    Urgent = 'urgent',
}
