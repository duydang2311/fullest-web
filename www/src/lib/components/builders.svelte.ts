import * as menu from '@zag-js/menu';
import { normalizeProps, useMachine } from '@zag-js/svelte';
import * as tabs from '@zag-js/tabs';

export const createTabs = (props: tabs.Props) => {
	return new Tabs(props);
};

export const createMenu = (props: menu.Props) => {
	return new Menu(props);
};

export class Tabs {
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

class Menu {
	readonly #api: menu.Api;

	constructor(props: menu.Props) {
		// eslint-disable-next-line @typescript-eslint/no-explicit-any
		const service = useMachine(menu.machine as any, props) as menu.Service;
		this.#api = $derived(menu.connect(service, normalizeProps));
	}

	get open() {
		return this.#api.open;
	}

	get highlightedValue() {
		return this.#api.highlightedValue;
	}

	setOpen(open: boolean) {
		return this.#api.setOpen(open);
	}

	setHighlightedValue(value: string) {
		return this.#api.setHighlightedValue(value);
	}

	setParent(parent: Parameters<menu.Api['setParent']>[0]) {
		return this.#api.setParent(parent);
	}

	setChild(child: Parameters<menu.Api['setChild']>[0]) {
		return this.#api.setChild(child);
	}

	reposition(options?: Partial<menu.PositioningOptions>) {
		return this.#api.reposition(options);
	}

	getOptionItemState(props: menu.OptionItemProps) {
		return this.#api.getOptionItemState(props);
	}

	getItemState(props: menu.ItemProps) {
		return this.#api.getItemState(props);
	}

	addItemListener(props: menu.ItemListenerProps) {
		return this.#api.addItemListener(props);
	}

	getContextTriggerProps() {
		return this.#api.getContextTriggerProps();
	}

	getTriggerItemProps<A extends menu.Api>(childApi: A) {
		return this.#api.getTriggerItemProps(childApi);
	}

	getTriggerProps() {
		return this.#api.getTriggerProps();
	}

	getIndicatorProps() {
		return this.#api.getIndicatorProps();
	}

	getPositionerProps() {
		return this.#api.getPositionerProps();
	}

	getArrowProps() {
		return this.#api.getArrowProps();
	}

	getArrowTipProps() {
		return this.#api.getArrowTipProps();
	}

	getContentProps() {
		return this.#api.getContentProps();
	}

	getSeparatorProps() {
		return this.#api.getSeparatorProps();
	}

	getItemProps(options: menu.ItemProps) {
		return this.#api.getItemProps(options);
	}

	getOptionItemProps(option: menu.OptionItemProps) {
		return this.#api.getOptionItemProps(option);
	}

	getItemIndicatorProps(option: menu.ItemBaseProps) {
		return this.#api.getItemIndicatorProps(option);
	}

	getItemTextProps(option: menu.ItemBaseProps) {
		return this.#api.getItemTextProps(option);
	}

	getItemGroupLabelProps(options: menu.ItemGroupLabelProps) {
		return this.#api.getItemGroupLabelProps(options);
	}

	getItemGroupProps(options: menu.ItemGroupProps) {
		return this.#api.getItemGroupProps(options);
	}
}
