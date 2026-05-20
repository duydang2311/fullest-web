<script lang="ts">
    import { combine } from '@duydang2311/jsbelt';
    import { watch } from '@duydang2311/svutils';
    import {
        autoUpdate,
        computePosition,
        flip,
        offset,
        shift,
        type FloatingElement,
        type ReferenceElement,
    } from '@floating-ui/dom';
    import { type Editor, type EditorEvents } from '@tiptap/core';
    import { portal } from '@zag-js/svelte';
    import { boolAttr } from 'runed';
    import { tick } from 'svelte';
    import { guard, guardNull } from '../utils/guard';
    import { SlashCommandsExtension, type SlashCommandsExtensionStorage } from './editor.svelte';
    import {
        IconH1,
        IconH2,
        IconH3,
        IconH4,
        IconH5,
        IconH6,
        IconListOrdered,
        IconListUnordered,
        IconMinus,
        IconText,
    } from './icons';

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
            keywords: ['separator', 'hr', 'horizontal line'],
            icon: IconMinus,
            onSelect() {
                editor.commands.setHorizontalRule();
            },
        },
        {
            title: 'Bullet List',
            keywords: ['ul', 'bulleted list', 'unordered list', 'list'],
            icon: IconListUnordered,
            onSelect() {
                editor.commands.toggleBulletList();
            },
        },
        {
            title: 'Numbered List',
            keywords: ['ol', 'numbered list', 'ordered list', 'list'],
            icon: IconListOrdered,
            onSelect() {
                editor.commands.toggleOrderedList();
            },
        },
    ];

    let selectedIndex = $state.raw(0);
    let data = $state.raw<ReturnType<SlashCommandsExtensionStorage['manager']['data']> | null>(
        null
    );
    const query = $derived(data ? data.command.slice(1) : null);
    let coords = $state.raw<{ [k in 'top' | 'left' | 'right' | 'bottom']: number } | null>(null);
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
    const virtualElement = $derived(
        coords
            ? {
                  getBoundingClientRect() {
                      guardNull(coords);
                      return {
                          x: coords.left,
                          y: coords.bottom,
                          top: coords.top,
                          left: coords.left,
                          right: coords.right,
                          bottom: coords.bottom,
                          width: coords.right - coords.left,
                          height: coords.bottom - coords.top,
                      };
                  },
                  contextElement: editor.options.element as Element,
              }
            : null
    );

    function handleTransaction(e: EditorEvents['transaction']) {
        data = e.editor.storage.slash.manager.data();
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
        if (!options || selectedIndex >= options.length) {
            selectedIndex = 0;
        }
    });

    function updatePosition(refEl: ReferenceElement, floatEl: FloatingElement) {
        computePosition(refEl, floatEl, {
            placement: 'bottom-start',
            middleware: [offset(4), flip(), shift()],
        }).then(({ x, y }) => {
            floatEl.style.transform = `translate(${x}px, ${y}px)`;
        });
    }

    watch(() => data)(() => {
        coords = data ? editor.view.coordsAtPos(data.pos) : null;
    });

    watch(() => [menuEl])(() => {
        if (!menuEl) {
            return;
        }
        const virtualElement = {
            getBoundingClientRect() {
                return {
                    x: 0,
                    y: 0,
                    top: 0,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    width: 0,
                    height: 0,
                };
            },
            contextElement: editor.options.element as Element,
        };
        return autoUpdate(virtualElement, menuEl, () => {
            if (!menuEl || !data) {
                return;
            }
            const c = editor.view.coordsAtPos(data.pos);
            const virtualElement = {
                getBoundingClientRect() {
                    return {
                        x: c.left,
                        y: c.bottom,
                        top: c.top,
                        left: c.left,
                        right: c.right,
                        bottom: c.bottom,
                        width: c.right - c.left,
                        height: c.bottom - c.top,
                    };
                },
            };
            coords = c;
            updatePosition(virtualElement, menuEl);
        });
    });
</script>

{#if focused && options && options.length}
    <div use:portal bind:this={menuEl} class="absolute top-0 left-0">
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
