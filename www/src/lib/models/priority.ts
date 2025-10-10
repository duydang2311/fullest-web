import type { Project } from './project';

export interface Priority {
	id: string;
	name: string;
	color: string;
	rank: string;
	description?: string;
	projectId: string;
	project: Project;
}
