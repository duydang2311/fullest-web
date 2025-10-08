import { Editor, Extension, type EditorOptions } from '@tiptap/core';
import { ListKit } from '@tiptap/extension-list';
import { Placeholder } from '@tiptap/extensions';
import StarterKit from '@tiptap/starter-kit';

interface TextEditorOptions extends EditorOptions {
	placeholder?: string;
}

export function createTextEditor(options?: Partial<TextEditorOptions>) {
	return new TextEditor(options);
}

export class TextEditor {
	private state = $state.raw<{ current: Editor }>({ current: null! });
	public constructor(options?: Partial<TextEditorOptions>) {
		const extensions: Extension[] = [
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
		if (options?.placeholder != null) {
			extensions.push(
				Placeholder.configure({
					placeholder: options.placeholder,
				})
			);
			delete options.placeholder;
		}

		this.state = {
			current: new Editor({
				...options,
				extensions: [...extensions, ...(options?.extensions ?? [])],
				onTransaction: (props) => {
					this.state = { current: props.editor };
					options?.onTransaction?.(props);
				},
			}),
		};
	}

	get current() {
		return this.state.current;
	}
}
