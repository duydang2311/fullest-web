import { isPlainObject } from 'is-what';
import type { Project } from './project';
import type { User } from './user';

export enum NamespaceKind {
	None = 'none',
	User = 'user',
}

export interface UserNamespace {
	id: string;
	kind: 'user';
	userId: string;
	user: User;
	projects: Project[];
}

export type Namespace = UserNamespace;

export function isUserNamespace(obj: unknown): obj is UserNamespace {
	return isPlainObject(obj) && 'kind' in obj && obj.kind === 'user';
}