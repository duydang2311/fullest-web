import type { Label } from './label';
import type { Project } from './project';
import type { Status } from './status';
import type { User } from './user';

export interface Task {
	createdTime: string;
	updatedTime: string;
	id: string;
	projectId: string;
	project: Project;
	authorId: string;
	author: User;
	assignees: User[];
	publicId: string;
	title: string;
	statusId?: string;
	status?: Status;
	description?: string;
	dueTime?: string;
	dueTz?: string;
	deletedTime?: string;
	labels: Label[];
}
