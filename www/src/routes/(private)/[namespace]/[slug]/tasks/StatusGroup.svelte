<script lang="ts">
    import { page } from '$app/state';
    import { untrack } from 'svelte';
    import { createCollapsible } from '~/lib/components/builders.svelte';
    import { ChevronDownOutline } from '~/lib/components/icons';
    import { Flip, gsap } from '~/lib/utils/gsap';
    import type { LocalTask } from './utils';

    interface Props {
        name: string;
        items: LocalTask[];
    }

    gsap.registerPlugin(Flip);

    const { name, items }: Props = $props();
    const id = $props.id();
    const collapsible = createCollapsible({
        id,
        defaultOpen: untrack(() => items.length > 0),
    });
</script>

<div
    {...collapsible.api.getRootProps()}
    class="group rootEl col-span-full grid grid-cols-subgrid first:rounded-t-md last:rounded-b-md overflow-hidden"
>
    <button
        {...collapsible.api.getTriggerProps()}
        type="button"
        class="relative px-4 py-2 group-data-[state=open]:bg-surface-subtle hover:bg-surface-subtle active:bg-surface-emph transition col-span-full group-not-first:border-t group-not-first:border-t-surface-border group-data-[state=open]:border-b border-b-surface-border tracking-tight flex items-center gap-2"
    >
        <ChevronDownOutline
            class="transition-transform group-data-[state=closed]:rotate-180 -translate-x-full group-data-[state=open]:translate-x-0 group-data-[state=closed]:opacity-0 duration-200 ease-out"
        />
        <div
            class="group-data-[state=open]:font-medium text-fg-muted group-data-[state=open]:text-fg-emph transition -translate-x-6 group-data-[state=open]:translate-x-0 duration-200 ease-out"
        >
            {name}
        </div>
        <div
            class="absolute h-[calc(100%+2px)] w-1 bg-primary-fg scale-x-0 group-data-[state=open]:scale-x-100 transition-transform origin-left left-0 -top-px"
        ></div>
    </button>
    <div {...collapsible.api.getContentProps()} class="col-span-full grid grid-cols-subgrid">
        {#each items as item (item.id)}
            <a
                href="/{page.params.namespace}/{page.params.slug}/tasks/{item.publicId}"
                class="group/item relative col-span-full grid grid-cols-subgrid hover:bg-surface-subtle"
            >
                <div
                    class="absolute h-full w-1 group-hover/item:opacity-100 bg-primary-fg opacity-40 left-0 top-0 transition hover:duration-0"
                ></div>
                <div class="px-4 py-2 text-sm font-medium text-fg-muted content-center">
                    #{item.publicId}
                </div>
                <div class="px-4 py-2">{item.title}</div>
                <div class="px-4 py-2">Col 3</div>
                <div class="px-4 py-2">Col 4</div>
            </a>
        {/each}
    </div>
</div>
