import type { Project, ProjectMember } from './project';

export interface User {
    createdTime: string;
    id: string;
    name: string;
    deletedTime?: string | null;
    projects: Project[];
    projectMembers: ProjectMember[];
    displayName?: string |  null;
    imageKey?: string | null;
    imageVersion?: number;
}

export type UserPreset = {
    Avatar: Pick<User, 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
};
