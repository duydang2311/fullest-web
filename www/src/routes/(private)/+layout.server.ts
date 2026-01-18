import invariant from 'tiny-invariant';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async (e) => {
    invariant(e.locals.session, 'session must not be null');
    return { user: e.locals.session.user };
};
