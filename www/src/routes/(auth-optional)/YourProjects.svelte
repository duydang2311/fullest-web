<script lang="ts">
    import { goto } from '$app/navigation';
    import { page } from '$app/state';
    import { busy } from '@duydang2311/svutils';
    import { boolAttr } from 'runed';
    import invariant from 'tiny-invariant';
    import Avatar from '~/lib/components/Avatar.svelte';
    import { PlusOutline } from '~/lib/components/icons';
    import { keysetList, reverseKeysetList } from '~/lib/models/paginated';
    import { usePageData } from '~/lib/utils/kit';
    import { useRuntime } from '~/lib/utils/runtime';
    import { C } from '~/lib/utils/styles';
    import { fluentSearchParams } from '~/lib/utils/url';
    import type { PageData } from './$types';
    import { getProjectList, usePageContext } from './__page__/utils.svelte';

    const ctx = usePageContext();
    const data = usePageData<PageData>();
    invariant(data.projectList, 'data.projectList must not be null');
    const { http } = useRuntime();
    const listBusy = busy();

    async function loadMore(dir: 'forward' | 'backward') {
        invariant(data.session, 'data.session must not be null');
        listBusy.set();
        try {
            if (dir === 'forward') {
                const lastId = ctx.projectList.items.at(-1)!.id;
                const list = await getProjectList(http)(
                    data.session.user.id,
                    lastId,
                    null,
                    'asc',
                    5
                );
                ctx.projectList = keysetList({
                    items: [...ctx.projectList.items, ...list.items],
                    hasPrevious: ctx.projectList.hasPrevious,
                    hasNext: list.hasNext,
                    totalCount: list.totalCount,
                });
                await goto(
                    fluentSearchParams(page.url.searchParams)
                        .set('p', list.items.at(-1)?.id)
                        .toSearch(),
                    { replaceState: true }
                );
            } else {
                const lastId = ctx.projectList.items[0].id;
                const list = reverseKeysetList(
                    await getProjectList(http)(data.session.user.id, lastId, null, 'desc', 5)
                );
                ctx.projectList = keysetList({
                    items: [...list.items, ...ctx.projectList.items],
                    hasPrevious: list.hasPrevious,
                    hasNext: ctx.projectList.hasNext,
                    totalCount: list.totalCount,
                });
            }
        } finally {
            listBusy.unset();
        }
    }
</script>

<aside class="flex flex-col py-4">
    <div class="flex items-center justify-between px-2 lg:px-4">
        <div class="flex items-center gap-1">
            <!-- <div class="h-3 w-2 bg-fg-emph rounded-xs"></div> -->
            <h2 class="text-fg-muted items-center rounded-sm uppercase text-xs">Projects</h2>
        </div>
        <div>
            <a
                href="/new"
                class="{C.button({
                    variant: 'primary',
                    filled: true,
                })} flex items-center gap-2 text-body-sm px-2 py-1"
            >
                <PlusOutline />
                <span>New</span>
            </a>
        </div>
    </div>
    <div class="mt-2 px-1 flex-1 flex flex-col gap-2 overflow-hidden">
        {#if ctx.projectList}
            {#if ctx.projectList.hasPrevious}
                <div class="px-1 lg:px-3">
                    <button
                        type="button"
                        class="{C.button({
                            variant: 'base',
                            filled: true,
                            size: 'sm',
                        })} w-full"
                        disabled={listBusy.now}
                        onclick={() => {
                            loadMore('backward');
                        }}
                    >
                        Load more
                    </button>
                </div>
            {/if}
            <div
                data-busy={boolAttr(listBusy.now)}
                class="flex flex-col data-busy:animate-pulse overflow-auto px-1 max-h-full lg:px-3"
            >
                {#each ctx.projectList.items as item (item.id)}
                    <a
                        href="/{item.namespace.user.name}/{item.identifier}"
                        class="{C.button({
                            ghost: true,
                        })} flex not-hover:font-normal gap-4 items-center py-1"
                    >
                        <Avatar user={item.namespace.user} class="size-avatar-xs shrink-0" />
                        {item.name}
                    </a>
                {/each}
            </div>
            {#if ctx.projectList.hasNext}
                <div class="px-1 lg:px-3">
                    <button
                        type="button"
                        class="{C.button({
                            variant: 'base',
                            filled: true,
                            size: 'sm',
                        })} w-full"
                        disabled={listBusy.now}
                        onclick={() => {
                            loadMore('forward');
                        }}
                    >
                        Load more
                    </button>
                </div>
            {/if}
        {/if}
    </div>
</aside>
