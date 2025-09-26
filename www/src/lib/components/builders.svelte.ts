import * as tabs from '@zag-js/tabs';
import { useMachine, normalizeProps } from '@zag-js/svelte';

export const createTabs = (props: tabs.Props) => {
	return new Tabs(props);
};

class Tabs {
	readonly #api: tabs.Api;

	constructor(props: tabs.Props) {
		const service = useMachine(tabs.machine, props);
		this.#api = $derived(tabs.connect(service, normalizeProps));
	}

	public get value() {
		return this.#api.value;
	}

	public get focusedValue() {
		return this.#api.focusedValue;
	}

	public setValue(value: string) {
		return this.#api.setValue(value);
	}

	public clearValue() {
		return this.#api.clearValue();
	}

	public setIndicatorRect(value: string) {
		return this.#api.setIndicatorRect(value);
	}

	public syncTabIndex() {
		return this.#api.syncTabIndex();
	}

	public focus() {
		return this.#api.focus();
	}

	public selectNext(fromValue?: string) {
		return this.#api.selectNext(fromValue);
	}

	public selectPrev(fromValue?: string) {
		return this.#api.selectPrev(fromValue);
	}

	public getTriggerState(props: tabs.TriggerProps) {
		return this.#api.getTriggerState(props);
	}

	public getRootProps() {
		return this.#api.getRootProps();
	}

	public getListProps() {
		return this.#api.getListProps();
	}

	public getTriggerProps(props: tabs.TriggerProps) {
		return this.#api.getTriggerProps(props);
	}

	public getContentProps(props: tabs.ContentProps) {
		return this.#api.getContentProps(props);
	}

	public getIndicatorProps() {
		return this.#api.getIndicatorProps();
	}
}
