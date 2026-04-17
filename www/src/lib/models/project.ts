import type { Namespace } from './namespace';
import type { Role } from './role';
import type { Tag } from './tag';
import type { User } from './user';

export interface Project {
	createdTime: string;
	namespaceId: string;
	namespace: Namespace;
	id: string;
	name: string;
	summary: string | null;
	descriptionJson: string | null;
	identifier: string;
	deletedTime: string | null;
	projectMembers: string;
	tags: Tag[];
}

export interface ProjectMember {
	createdTime: string;
	id: string;
	projectId: string;
	project: Project;
	userId: string;
	user: User;
	roleId: string;
	role: Role;
}
