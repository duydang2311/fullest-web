<script lang="ts">
    import { page } from '$app/state';
    import { usePageData } from '~/lib/utils/kit';
    import type { PageData } from '../$types';
    import { C } from '~/lib/utils/styles';

    const pageData = usePageData<PageData>();
</script>

<div class="flex items-center gap-1 text-sm">
    <a
        href="/{page.params.namespace}"
        class="{C.button({ ghost: true, size: 'sm' })} not-hover:text-fg-dim"
    >
        {pageData.namespace.user.displayName ?? pageData.namespace.user.name}
    </a>
    <span class="text-fg-muted">/</span>
    <a
        href="/{page.params.namespace}/{page.params.slug}"
        class="{C.button({ ghost: true, size: 'sm' })} not-hover:text-fg-dim"
    >
        {page.params.slug}
    </a>
</div>
