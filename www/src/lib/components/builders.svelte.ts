import { watch } from '@duydang2311/svutils';
import * as collapsible from '@zag-js/collapsible';
import * as dialog from '@zag-js/dialog';
import * as fileUpload from '@zag-js/file-upload';
import * as listbox from '@zag-js/listbox';
import * as menu from '@zag-js/menu';
import * as popover from '@zag-js/popover';
import { normalizeProps, useMachine } from '@zag-js/svelte';
import * as tabs from '@zag-js/tabs';
import * as zagTooltip from '@zag-js/tooltip';
import { mount, unmount } from 'svelte';
import Tooltip from './Tooltip.svelte';

export function createTabs(props: tabs.Props) {
    return new Tabs(props);
}

export function createMenu(props: menu.Props) {
    return new Menu(props);
}

export function createFileUpload(props: fileUpload.Props) {
    return new FileUpload(props);
}

export function createCollapsible(props: collapsible.Props) {
    const service = useMachine(collapsible.machine, props);
    const api = $derived(collapsible.connect(service, normalizeProps));
    return {
        get api() {
            return api;
        },
    };
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

export function createTooltip(props: zagTooltip.Props) {
    const service = useMachine(zagTooltip.machine, props);
    const api = $derived(zagTooltip.connect(service, normalizeProps));
    return {
        get api() {
            return api;
        },
    };
}

export function tooltip(content: string) {
    return (node: HTMLElement) => {
        const componentInstance = mount(Tooltip, {
            target: node.parentElement ?? node,
            props: {
                content,
            },
        });
        const t = componentInstance.getTooltip();
        watch(() => t.api)(() => {
            const props = t.api.getTriggerProps();
            const listeners: [string, EventListener][] = [];

            for (const [key, value] of Object.entries(props)) {
                if (key.startsWith('on') && typeof value === 'function') {
                    const event = key.slice(2).toLowerCase();
                    node.addEventListener(event, value as EventListener);
                    listeners.push([event, value as EventListener]);
                } else {
                    node.setAttribute(key, String(value));
                }
            }
            return () => {
                for (const [event, handler] of listeners) {
                    node.removeEventListener(event, handler);
                }
            };
        });
        return () => {
            unmount(componentInstance, { outro: true });
        };
    };
}

export function createDialog(props: dialog.Props) {
    const service = useMachine(dialog.machine, props);
    const api = $derived(dialog.connect(service, normalizeProps));
    return {
        get api() {
            return api;
        },
    };
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
