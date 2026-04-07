<script lang="ts">
    import { page } from '$app/state';
    import { CheckCircleOutline } from '~/lib/components/icons';
    import About from './About.svelte';
    import ProjectActions from './ProjectActions.svelte';

    const { data } = $props();
</script>

<h1 class="sr-only">
    {data.namespace.kind === 'user' ? data.namespace.user.name : ''}/{data.project.name}
</h1>
<div class="max-w-container-xl mx-auto w-full *:px-8 py-4">
    <div class="border-base-border">
        {#if data.isCreatedJustNow}
            <div
                class="text-positive bg-positive/5 border border-positive/10 p-2 rounded-md c-help-text mb-8 flex items-center gap-2"
            >
                <CheckCircleOutline />
                <span>Project created successfully.</span>
            </div>
        {/if}
        <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
                <div class="size-avatar-sm bg-base-subtle rounded-full"></div>
                <a
                    href="/{page.params.namespace}/{page.params.slug}"
                    class="c-link text-title-lg text-title-xs font-semibold tracking-tight"
                >
                    {data.project.name}
                </a>
            </div>
            <div>
                <ProjectActions />
            </div>
        </div>
        {#if data.project.summary && data.project.summary.length > 0}
            <p class="max-w-container-md text-base-fg-dim mt-2">
                {data.project.summary}
            </p>
        {/if}
    </div>
    <div class="mt-4">
        <About project={data.project} />
    </div>
</div>
