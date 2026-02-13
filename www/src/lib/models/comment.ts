import type { User } from './user';

export interface Comment {
	createdTime: string;
	taskId: string;
	task: string;
	id: string;
	authorId: string;
	author: User;
	contentJson: string | null;
	contentPreview: string | null;
	deletedTime: string | null;
}
