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
	summary?: string;
	about?: string;
	identifier: string;
	deletedTime?: string;
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

export interface ProjectStatus {
	id: string;
	projectId: string;
	project: string;
	statusId: string;
	status: string;
	isDefault: boolean;
}
