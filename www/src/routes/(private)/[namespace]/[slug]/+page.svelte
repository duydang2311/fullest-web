<script lang="ts">
    import { page } from '$app/state';
    import Avatar from '~/lib/components/Avatar.svelte';
    import { CheckCircleOutline } from '~/lib/components/icons';
    import Breadcrumbs from './__page__/Breadcrumbs.svelte';
    import Description from './Description.svelte';
    import ProjectActions from './ProjectActions.svelte';

    const { data } = $props();
</script>

<div class="py-4">
    <h1 class="sr-only">
        {data.namespace.kind === 'user' ? data.namespace.user.name : ''}/{data.project.name}
    </h1>
    <div class="border-b border-b-surface-border pb-4 px-8">
        {#if data.isCreatedJustNow}
            <div
                class="text-positive bg-positive/5 border border-positive/10 p-2 rounded-md c-help-text mb-8 flex items-center gap-2"
            >
                <CheckCircleOutline />
                <span>Project created successfully.</span>
            </div>
        {/if}
        <div class="flex items-start justify-between">
            <div>
                <Breadcrumbs />
                <div class="flex items-center gap-2 mt-1">
                    <Avatar user={data.namespace.user} class="size-avatar-sm shrink-0" />
                    <a
                        href="/{page.params.namespace}/{page.params.slug}"
                        class="c-link text-fg-emph font-medium"
                    >
                        {data.project.name}
                    </a>
                </div>
            </div>
            <div>
                <ProjectActions />
            </div>
        </div>
        {#if data.project.summary && data.project.summary.length > 0}
            <p class="max-w-container-md text-fg-muted text-sm mt-2">
                {data.project.summary}
            </p>
        {/if}
    </div>
    <div class="mt-4 px-8">
        <Description />
    </div>
</div>
