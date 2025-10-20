import { Editor, type EditorOptions, type Extensions, type JSONContent } from '@tiptap/core';
import { ListKit } from '@tiptap/extension-list';
import { Placeholder } from '@tiptap/extensions';
import StarterKit from '@tiptap/starter-kit';
import { renderToHTMLString as __renderToHTMLString } from '@tiptap/static-renderer';

interface TextEditorOptions extends EditorOptions {
	placeholder?: string;
}

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

export function renderToHTMLString(jsonContent: JSONContent) {
	return __renderToHTMLString({ content: jsonContent, extensions: createExtensions() });
}
