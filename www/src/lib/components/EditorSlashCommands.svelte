<script lang="ts">
    import { combine } from '@duydang2311/jsbelt';
    import { watch } from '@duydang2311/svutils';
    import { type Editor, type EditorEvents } from '@tiptap/core';
    import { portal } from '@zag-js/svelte';
    import { boolAttr } from 'runed';
    import { guard, guardNull } from '../utils/guard';
    import { SlashCommandsExtension } from './editor.svelte';
    import { IconH1, IconH2, IconH3, IconH4, IconH5, IconH6, IconMinus, IconText } from './icons';

    const { editor }: { editor: Editor } = $props();
    const BASE_OPTIONS = [
        {
            title: 'Paragraph',
            keywords: ['text', 'p', 'paragraph'],
            icon: IconText,
            onSelect() {
                editor.commands.setParagraph();
            },
        },
        {
            title: 'Heading 1',
            keywords: ['heading 1', 'h1', 'title'],
            icon: IconH1,
            onSelect() {
                editor.commands.setHeading({ level: 1 });
            },
        },
        {
            title: 'Heading 2',
            keywords: ['heading 2', 'h2', 'subtitle'],
            icon: IconH2,
            onSelect() {
                editor.commands.setHeading({ level: 2 });
            },
        },
        {
            title: 'Heading 3',
            keywords: ['heading 3', 'h3'],
            icon: IconH3,
            onSelect() {
                editor.commands.setHeading({ level: 3 });
            },
        },
        {
            title: 'Heading 4',
            keywords: ['heading 4', 'h4'],
            icon: IconH4,
            onSelect() {
                editor.commands.setHeading({ level: 4 });
            },
        },
        {
            title: 'Heading 5',
            keywords: ['heading 5', 'h5'],
            icon: IconH5,
            onSelect() {
                editor.commands.setHeading({ level: 5 });
            },
        },
        {
            title: 'Heading 6',
            keywords: ['heading 6', 'h6'],
            icon: IconH6,
            onSelect() {
                editor.commands.setHeading({ level: 6 });
            },
        },
        {
            title: 'Separator',
            keywords: ['separator', 'hr', 'horizontal line', 'horizontal rule'],
            icon: IconMinus,
            onSelect() {
                editor.commands.setHorizontalRule();
            },
        },
    ];

    let selectedIndex = $state.raw(0);
    let query = $state.raw<string | null>(null);
    let coords = $state.raw<{ [k in 'left' | 'top' | 'right' | 'bottom']: number } | null>(null);
    let focused = $state.raw(false);
    let menuEl = $state.raw<HTMLDivElement>();
    const options = $derived(
        query != null
            ? query.length > 0
                ? BASE_OPTIONS.filter((a) =>
                      a.keywords.some((b) => {
                          guardNull(query);
                          return b.includes(query);
                      })
                  )
                : BASE_OPTIONS
            : null
    );

    function handleTransaction(e: EditorEvents['transaction']) {
        const data = e.editor.storage.slash.manager.data();
        if (data) {
            query = data.command.slice(1);
            coords = editor.view.coordsAtPos(data.pos);
        } else {
            selectedIndex = 0;
            query = null;
            coords = null;
        }
    }

    function handleFocus() {
        focused = true;
    }

    function handleBlur(e: EditorEvents['blur']) {
        if (
            !e.event.relatedTarget ||
            !(e.event.relatedTarget instanceof Node) ||
            !menuEl?.contains(e.event.relatedTarget)
        ) {
            focused = false;
        }
    }

    function select(option: { onSelect: () => void }) {
        const data = editor.storage.slash.manager.data();
        guardNull(data);
        editor.view.dispatch(editor.state.tr.delete(data.pos, data.pos + data.command.length));
        option.onSelect();
    }

    watch(() => editor)(() => {
        guard(
            editor.extensionManager.extensions.some((a) => a.name === SlashCommandsExtension.name),
            'SlashCommandsExtension must be registered'
        );
        const manager = editor.storage.slash.manager;
        editor.on('transaction', handleTransaction);
        editor.on('focus', handleFocus);
        editor.on('blur', handleBlur);
        const cleanup = combine(
            manager.on('enter', (e) => {
                if (!options || !options.length) {
                    e.preventDefault();
                    return;
                }
                const selectedOption = options[selectedIndex];
                if (!selectedOption) {
                    e.preventDefault();
                    return;
                }
                select(selectedOption);
            }),
            manager.on('arrow-up', (e) => {
                if (!options || options.length <= 1) {
                    e.preventDefault();
                    return;
                }
                selectedIndex = selectedIndex === 0 ? options.length - 1 : selectedIndex - 1;
            }),
            manager.on('arrow-down', (e) => {
                if (!options || options.length <= 1) {
                    e.preventDefault();
                    return;
                }
                selectedIndex = (selectedIndex + 1) % options.length;
            }),
            () => {
                editor.off('transaction', handleTransaction);
                editor.off('focus', handleFocus);
                editor.off('blur', handleBlur);
            }
        );
        return cleanup;
    });

    watch(() => options)(() => {
        if (!options) {
            return;
        }
        if (selectedIndex >= options.length) {
            selectedIndex = 0;
        }
    });
</script>

{#if focused && coords && options && options.length}
    <div
        use:portal
        bind:this={menuEl}
        class="fixed top-0 left-0"
        style="transform: translate({coords.left}px, calc(0.25rem + {coords.bottom}px));"
    >
        <ul class="c-menu-content min-w-40">
            {#each options as option, i (option.title)}
                <li>
                    <button
                        type="button"
                        class="c-menu-item flex items-center gap-4 w-full"
                        data-highlighted={boolAttr(i === selectedIndex)}
                        onclick={() => {
                            select(option);
                        }}
                    >
                        <option.icon />
                        <span>{option.title}</span>
                    </button>
                </li>
            {/each}
        </ul>
    </div>
{/if}
