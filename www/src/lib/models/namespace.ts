import type { Project } from './project';
import type { User } from './user';

export enum NamespaceKind {
	None = 'none',
	User = 'user',
}

export interface Namespace {
	id: string;
	kind: 'user';
	userId: string;
	user: User;
	projects: Project[];
}
