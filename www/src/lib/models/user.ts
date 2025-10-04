import type { Project, ProjectMember } from './project';

export interface User {
	createdTime: string;
	id: string;
	name: string;
	// auths: string;
	deletedTime?: string;
	projects: Project[];
	projectMembers: ProjectMember[];
}
