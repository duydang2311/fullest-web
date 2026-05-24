<script lang="ts">
    import { goto } from '$app/navigation';
    import { page } from '$app/state';
    import { busy } from '@duydang2311/svutils';
    import { boolAttr } from 'runed';
    import invariant from 'tiny-invariant';
    import Avatar from '~/lib/components/Avatar.svelte';
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

<div class="flex-1 flex flex-col gap-2 overflow-hidden -m-4">
    {#if ctx.projectList}
        {#if ctx.projectList.hasPrevious}
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
        {/if}
        <ol
            data-busy={boolAttr(listBusy.now)}
            class="flex flex-col data-busy:animate-pulse overflow-auto max-h-full divide-y divide-surface-border"
        >
            {#each ctx.projectList.items as item (item.id)}
                <li class="group">
                    <a
                        href="/{item.namespace.user.name}/{item.identifier}"
                        class="flex gap-4 items-center px-4 py-2 hover:bg-base-subtle active:bg-base-emph transition group-last:rounded-b-lg hover:duration-0"
                    >
                        <Avatar user={item.namespace.user} class="size-avatar-sm shrink-0" />
                        <div>
                            <div class="font-medium">{item.name}</div>
                            <div class="text-fg-muted text-body-xs">
                                <span>{item.identifier}</span>
                                <span>·</span>
                                <span>Today</span>
                            </div>
                        </div>
                    </a>
                </li>
            {/each}
        </ol>
        {#if ctx.projectList.hasNext}
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
        {/if}
    {/if}
</div>
