import { createNanoEvents } from '@duydang2311/jsbelt';
import type { InlineEdit } from '@duydang2311/sveltecraft';
import {
    Editor,
    Extension,
    type EditorEvents,
    type EditorOptions,
    type Extensions,
    type JSONContent,
} from '@tiptap/core';
import { ListKit } from '@tiptap/extension-list';
import { TableKit } from '@tiptap/extension-table';
import { Placeholder } from '@tiptap/extensions';
import { Markdown } from '@tiptap/markdown';
import type { ResolvedPos } from '@tiptap/pm/model';
import { TextSelection } from '@tiptap/pm/state';
import StarterKit from '@tiptap/starter-kit';
import { renderToHTMLString as __renderToHTMLString } from '@tiptap/static-renderer';
import { untrack } from 'svelte';
import type { Attachment } from 'svelte/attachments';

interface TextEditorOptions extends EditorOptions {
    placeholder?: string;
}

export type SlashCommandsExtensionStorage = {
    manager: SlashStorageManager;
};

declare module '@tiptap/core' {
    interface Storage {
        slash: SlashCommandsExtensionStorage;
    }
}

export type SvelteEditor = Attachment<HTMLElement> & { current: Editor | null };

export function createExtensions(): Extensions {
    return [
        StarterKit.configure({
            listItem: false,
            bulletList: false,
            listKeymap: false,
            orderedList: false,
        }),
        ListKit.configure({
            taskItem: {
                nested: true,
            },
        }),
        TableKit,
        Markdown,
        MoveNodeVerticallyExtension,
    ];
}

export function createTextEditor(options?: Partial<TextEditorOptions>) {
    const extensions = createExtensions();
    if (options?.placeholder != null) {
        extensions.push(
            Placeholder.configure({
                placeholder: options.placeholder,
            })
        );
        delete options.placeholder;
    }

    return new Editor({
        ...options,
        extensions: [...extensions, ...(options?.extensions ?? [])],
    });
}

export function createEditor(options?: Omit<Partial<TextEditorOptions>, 'element'>) {
    let editor = $state.raw<[Editor | null]>([null]);
    const extensions = createExtensions();
    if (options?.placeholder != null) {
        extensions.push(
            Placeholder.configure({
                placeholder: options.placeholder,
            })
        );
        delete options.placeholder;
    }
    const attachment: Attachment<HTMLElement> = (element: HTMLElement) => {
        const e = new Editor({
            ...options,
            element,
            extensions: [...extensions, ...(options?.extensions ?? [])],
        });
        untrack(() => {
            editor = [e];
        });
        return () => {
            e.destroy();
        };
    };
    return Object.defineProperties(attachment, {
        current: {
            get() {
                return editor[0];
            },
        },
    }) as SvelteEditor;
}

export function renderToHTMLString(jsonContent: JSONContent) {
    return __renderToHTMLString({ content: jsonContent, extensions: createExtensions() });
}

export const SubmitShortcutExtension = Extension.create<{ onSubmit: VoidFunction }>({
    name: 'submitShortcutExtension',
    addKeyboardShortcuts() {
        return {
            'Ctrl-Enter': () => {
                this.options.onSubmit();
                return true;
            },
        };
    },
});

export const DisableInlineEditShortcutExtension = Extension.create<{ edit: InlineEdit }>({
    name: 'disableInlineEditShortcutExtension',
    addKeyboardShortcuts() {
        return {
            Escape: () => {
                this.options.edit.enabled = false;
                return true;
            },
        };
    },
});

function findMoveDepth(from: ResolvedPos) {
    for (let d = from.depth; d > 0; d--) {
        if (from.node(d).type.name === 'listItem') {
            return d;
        }
    }

    for (let d = from.depth; d > 0; d--) {
        if (from.node(d).isBlock) {
            return d;
        }
    }

    return 1;
}

export const MoveNodeVerticallyExtension = Extension.create<{ edit: InlineEdit }>({
    name: 'moveNodeVerticallyExtension',
    addKeyboardShortcuts() {
        return {
            'Alt-ArrowUp': ({ editor }) => {
                const { state, view } = editor;
                const { selection } = state;
                const { $from: from, $to: to } = selection;
                const depth = findMoveDepth(from);
                const node = from.node(depth);
                const parent = from.node(depth - 1);
                const startIndex = from.index(depth - 1);
                if (startIndex === 0) {
                    return true;
                }

                if (selection.empty) {
                    const pos = from.before(depth);
                    const prevNode = parent.child(startIndex - 1);
                    const tr = state.tr;
                    tr.delete(pos, pos + node.nodeSize);
                    const insertPos = tr.mapping.map(pos - prevNode.nodeSize);
                    tr.insert(insertPos, node);
                    tr.setSelection(TextSelection.near(tr.doc.resolve(insertPos + 1)));
                    view.dispatch(tr);
                    return true;
                }

                const toDepth = findMoveDepth(to);

                // must match movable level
                if (depth !== toDepth) {
                    return true;
                }

                // must share same parent
                if (from.start(depth - 1) !== to.start(depth - 1)) {
                    return true;
                }

                const rangeStart = from.before(depth);
                const rangeEnd = to.after(depth);
                const beforeNode = parent.child(startIndex - 1);
                const slice = state.doc.slice(rangeStart, rangeEnd);
                const tr = state.tr;
                tr.delete(rangeStart, rangeEnd);
                const insertPos = tr.mapping.map(rangeStart - beforeNode.nodeSize);
                tr.insert(insertPos, slice.content);
                const movedSize = slice.content.size;
                tr.setSelection(
                    TextSelection.create(tr.doc, insertPos + 1, insertPos + movedSize - 1)
                );
                view.dispatch(tr);
                return true;
            },
            'Alt-ArrowDown': ({ editor }) => {
                const { state, view } = editor;
                const { selection } = state;
                const { $from: from, $to: to } = selection;
                const depth = findMoveDepth(from);
                const node = from.node(depth);
                const parent = from.node(depth - 1);
                const startIndex = from.index(depth - 1);
                if (selection.empty) {
                    // already last sibling
                    if (startIndex >= parent.childCount - 1) {
                        return true;
                    }

                    // careful of last empty <p>
                    const nextNode =
                        startIndex <= parent.childCount - 2 ? parent.child(startIndex + 1) : null;
                    if (
                        nextNode == null ||
                        (nextNode.type.name === 'paragraph' && nextNode.content.size === 0)
                    ) {
                        return true;
                    }

                    const pos = from.before(depth);
                    const tr = state.tr;
                    tr.delete(pos, pos + node.nodeSize);
                    const insertPos = tr.mapping.map(from.after(depth) + nextNode.nodeSize);
                    tr.insert(insertPos, node);
                    tr.setSelection(TextSelection.near(tr.doc.resolve(insertPos + 1)));
                    view.dispatch(tr);
                    return true;
                }

                const toDepth = findMoveDepth(to);

                // must match movable level
                if (depth !== toDepth) {
                    return true;
                }

                // must share same parent
                if (from.start(depth - 1) !== to.start(depth - 1)) {
                    return true;
                }

                const endIndex = to.index(depth - 1);

                // already last sibling
                if (endIndex >= parent.childCount - 1) {
                    return true;
                }

                // careful of last empty <p>
                const nextNode =
                    endIndex <= parent.childCount - 2 ? parent.child(endIndex + 1) : null;
                if (
                    nextNode == null ||
                    (nextNode.type.name === 'paragraph' && nextNode.content.size === 0)
                ) {
                    return true;
                }

                const rangeStart = from.before(depth);
                const rangeEnd = to.after(depth);
                const slice = state.doc.slice(rangeStart, rangeEnd);
                const tr = state.tr;
                tr.delete(rangeStart, rangeEnd);
                const insertPos = tr.mapping.map(rangeEnd + nextNode.nodeSize);
                tr.insert(insertPos, slice.content);
                tr.setSelection(
                    TextSelection.create(tr.doc, insertPos + 1, insertPos + slice.size - 1)
                );
                view.dispatch(tr);
                return true;
            },
        };
    },
});

const CHAR_CODE_WHITESPACE = ' '.charCodeAt(0);
const CHAR_CODE_SLASH = '/'.charCodeAt(0);
export const SlashCommandsExtension = Extension.create<any, SlashCommandsExtensionStorage>({
    name: 'slash',
    addStorage() {
        return { manager: new SlashStorageManager() };
    },
    addKeyboardShortcuts() {
        return this.editor.storage.slash.manager.addKeyboardShortcuts();
    },
    onTransaction(e) {
        return this.storage.manager.onTransaction(e);
    },
});

type SlashStorageManagerEvents = {
    enter: (e: CustomEvent<never>) => void;
    'arrow-up': (e: CustomEvent<never>) => void;
    'arrow-down': (e: CustomEvent<never>) => void;
};
class SlashStorageManager {
    #data: { command: string; pos: number } | null = null;
    #index = 0;
    #emitter = createNanoEvents<SlashStorageManagerEvents>();

    data() {
        return this.#data;
    }

    selectedIndex() {
        return this.#index;
    }

    on<T extends keyof SlashStorageManagerEvents>(
        event: T,
        callback: SlashStorageManagerEvents[T]
    ) {
        return this.#emitter.on(event, callback);
    }

    addKeyboardShortcuts() {
        return {
            Enter: () => {
                if (!this.#data) {
                    return false;
                }

                const e = new CustomEvent<never>('enter', { cancelable: true });
                this.#emitter.emit('enter', e);
                console.log(e.defaultPrevented);
                return !e.defaultPrevented;
            },
            ArrowUp: () => {
                if (!this.#data) {
                    return false;
                }
                const e = new CustomEvent<never>('arrow-up', { cancelable: true });
                this.#emitter.emit('arrow-up', e);
                return !e.defaultPrevented;
            },
            ArrowDown: () => {
                if (!this.#data) {
                    return false;
                }
                const e = new CustomEvent<never>('arrow-down', { cancelable: true });
                this.#emitter.emit('arrow-down', e);
                return !e.defaultPrevented;
            },
        };
    }

    onTransaction(e: EditorEvents['transaction']) {
        const editor = e.editor;
        const { $from: from } = editor.state.selection;
        const content = from.parent.textBetween(0, from.parentOffset);
        let index = -1;
        let last = -1;
        for (let i = 0, size = content.length; i !== size; ++i) {
            const current = content.charCodeAt(i);
            if (current === CHAR_CODE_SLASH && (i === 0 || last === CHAR_CODE_WHITESPACE)) {
                index = i;
                break;
            }
            last = current;
        }
        if (index === -1) {
            this.#data = null;
            return;
        }
        this.#data = {
            command: content.slice(index),
            pos: from.start() + index,
        };
    }
}
