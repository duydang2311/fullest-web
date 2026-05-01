import {
    Editor,
    Extension,
    type EditorOptions,
    type Extensions,
    type JSONContent,
} from '@tiptap/core';
import { ListKit } from '@tiptap/extension-list';
import { TableKit } from '@tiptap/extension-table';
import { Placeholder } from '@tiptap/extensions';
import { Markdown } from '@tiptap/markdown';
import { Slice } from '@tiptap/pm/model';
import { Plugin, PluginKey } from '@tiptap/pm/state';
import StarterKit from '@tiptap/starter-kit';
import { renderToHTMLString as __renderToHTMLString } from '@tiptap/static-renderer';
import { untrack } from 'svelte';
import type { Attachment } from 'svelte/attachments';

interface TextEditorOptions extends EditorOptions {
    placeholder?: string;
}

export type SvelteEditor = Attachment<HTMLElement> & { current: Editor };

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
        MarkdownClipboard,
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
    let editor = $state.raw<{ current: Editor }>({ current: null! });
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
        let e = new Editor({
            ...options,
            element,
            extensions: [...extensions, ...(options?.extensions ?? [])],
            onTransaction(props) {
                editor = { current: props.editor };
                options?.onTransaction?.(props);
            },
        });
        untrack(() => {
            editor = { current: e };
        });
        return () => {
            e.destroy();
        };
    };
    return Object.defineProperties(attachment, {
        current: {
            get() {
                return editor.current;
            },
            set(value) {
                editor = { current: value };
            },
            enumerable: true,
        },
    }) as SvelteEditor;
}

export function renderToHTMLString(jsonContent: JSONContent) {
    return __renderToHTMLString({ content: jsonContent, extensions: createExtensions() });
}

export const MarkdownClipboard = Extension.create({
    name: 'markdownClipboard',
    addOptions: () => ({
        transformPastedText: true,
        transformCopiedText: true,
    }),
    addProseMirrorPlugins() {
        return [
            new Plugin({
                key: new PluginKey('markdownClipboard'),
                props: {
                    clipboardTextParser: (text, _context, plainText) => {
                        if (
                            plainText ||
                            !this.options.transformPastedText ||
                            !this.editor.markdown
                        ) {
                            return null as any;
                        }
                        const parsedJson = this.editor.markdown.parse(text);
                        const doc = this.editor.schema.nodeFromJSON(parsedJson);
                        return new Slice(doc.content, 1, 1);
                    },

                    clipboardTextSerializer: (slice) => {
                        if (!this.options.transformCopiedText || !this.editor.markdown) {
                            return null as any;
                        }
                        const sliceJson = slice.toJSON();
                        sliceJson.type = 'doc';
                        const md = this.editor.markdown.serialize(sliceJson);
                        return md;
                    },
                },
            }),
        ];
    },
});

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
