import { AsyncLocalStorage } from 'node:async_hooks';

type Runtime = App.Locals;

const storage = new AsyncLocalStorage<Runtime>();

export const withRuntime =
	(locals: Runtime) =>
	<T, TArgs extends unknown[]>(f: (...args: TArgs) => MaybePromise<T>, ...args: TArgs) => {
		return storage.run(locals, () => f(...args));
	};

export const useRuntime = () => {
	const store = storage.getStore();
	if (!store) {
		throw new Error('useRuntime() called outside withRuntime');
	}
	return store;
};
