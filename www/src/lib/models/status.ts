export interface Status {
	id: string;
	name: string;
	category: StatusCategory;
	color: string;
	description?: string;
}

export enum StatusCategory {
	None = 'none',
	Pending = 'pending',
	Active = 'active',
	Completed = 'completed',
	Canceled = 'canceled',
}
