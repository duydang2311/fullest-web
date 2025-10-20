import { stringify } from 'node:querystring';

export const withQueryParams = (url: string, params: Record<string, number | string>) => {
	url += (url.includes('?') ? '&' : '?') + stringify(params);
	return url;
};
