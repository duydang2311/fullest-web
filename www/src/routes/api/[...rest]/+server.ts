import { type RequestHandler } from '@sveltejs/kit';

export const fallback: RequestHandler = (e) => {
    throw new Error('should not be called');
};
