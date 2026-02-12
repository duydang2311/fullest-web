import * as fileUpload from '@zag-js/file-upload';
import * as listbox from '@zag-js/listbox';
import * as menu from '@zag-js/menu';
import * as popover from '@zag-js/popover';
import { normalizeProps, useMachine } from '@zag-js/svelte';
import * as tabs from '@zag-js/tabs';

export function createTabs(props: tabs.Props) {
    return new Tabs(props);
}

export function createMenu(props: menu.Props) {
    return new Menu(props);
}

export function createFileUpload(props: fileUpload.Props) {
    return new FileUpload(props);
}

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

    get api() {
        return this.#api;
    }
}

export function createListbox<T>(props: listbox.Props<T>) {
    return new Listbox(props);
}

export function createPopover(props: popover.Props) {
    return new Popover(props);
}

class FileUpload {
    readonly #api: fileUpload.Api;

    constructor(props: fileUpload.Props) {
        const service = useMachine(fileUpload.machine as never, props) as fileUpload.Service;
        this.#api = $derived(fileUpload.connect(service, normalizeProps));
    }

    get api() {
        return this.#api;
    }
}

class Listbox {
    readonly #api: listbox.Api;

    constructor(props: listbox.Props) {
        const service = useMachine(listbox.machine as any, props) as listbox.Service;
        this.#api = $derived(listbox.connect(service, normalizeProps));
    }

    get api() {
        return this.#api;
    }
}

class Popover {
    readonly #api: popover.Api;

    constructor(props: popover.Props) {
        const service = useMachine(popover.machine as any, props) as popover.Service;
        this.#api = $derived(popover.connect(service, normalizeProps));
    }

    get api() {
        return this.#api;
    }
}
