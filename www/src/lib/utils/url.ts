import { stringify } from 'node:querystring';

export const withQueryParams = (url: string, params: Record<string, string>) => {
	url += (url.includes('?') ? '&' : '?') + stringify(params);
	return url;
};
