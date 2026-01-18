import type { Project, ProjectMember } from './project';

export interface User {
    createdTime: string;
    id: string;
    name: string;
    deletedTime?: string;
    projects: Project[];
    projectMembers: ProjectMember[];
    displayName?: string;
    imageKey?: string;
    imageVersion?: number;
}

export type UserPreset = {
    Avatar: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
};
