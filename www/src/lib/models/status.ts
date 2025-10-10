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
	None = 'none',
	Pending = 'pending',
	Active = 'active',
	Completed = 'completed',
	Canceled = 'canceled',
}
