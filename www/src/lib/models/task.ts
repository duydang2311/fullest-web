import type { Label } from './label';
import type { Priority } from './priority';
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
	priorityId?: string;
	priority?: Priority;
	initialCommentId: string;
	initialComment: Comment;
	comments?: Comment[];
	dueTime?: string;
	dueTz?: string;
	deletedTime?: string;
	labels: Label[];
}
