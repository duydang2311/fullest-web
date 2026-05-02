import { error as __error } from '@sveltejs/kit';
import { isHttpErr, isHttpValidationErr } from './errors';

export function error(error: App.Error) {
    return __error(isHttpErr(error) || isHttpValidationErr(error) ? error.status : 500, error);
}
