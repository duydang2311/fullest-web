<script lang="ts">
    import { page } from '$app/state';
    import type { Snippet } from 'svelte';
    import { usePageData } from '~/lib/utils/kit';
    import type { PageData } from '../$types';

    const pageData = usePageData<PageData>();
</script>

<div class="text-sm text-fg-dim">
    <a href="/{page.params.namespace}" class="c-link">
        {pageData.namespace.user.displayName ?? pageData.namespace.user.name}
    </a>
    <span>/</span>
    <a href="/{page.params.namespace}/{page.params.slug}" class="c-link">
        {page.params.slug}
    </a>
</div>
