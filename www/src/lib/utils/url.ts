export function namespaceUrl(namespace: string) {
    return `/${namespace}`;
}

export function fluentSearchParams(searchParams: URLSearchParams) {
    return new FluentSearchParams(searchParams);
}

class FluentSearchParams {
    #searchParams: URLSearchParams;

    constructor(searchParams: URLSearchParams) {
        this.#searchParams = searchParams;
    }

    set(name: string, value: string | null | undefined) {
        if (value) {
            this.#searchParams.set(name, value);
        }
        return this;
    }

    delete(name: string) {
        this.#searchParams.delete(name);
        return this;
    }

    toSearch() {
        const search = this.#searchParams.toString();
        return search ? `?${search}` : search;
    }
}
