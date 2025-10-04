export interface Role {
	id: string;
	name: string;
	rank: string;
	permissions: Permission[];
}

export interface Permission {
	id: string;
	name: string;
}
