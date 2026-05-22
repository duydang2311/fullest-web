<script lang="ts">
    import { page } from '$app/state';
    import Breadcrumbs from './__page__/Breadcrumbs.svelte';
    import Description from './__page__/Description.svelte';
    import ProjectActions from './__page__/ProjectActions.svelte';
    import Team from './__page__/Team.svelte';
    import { useProject } from './__page__/utils.svelte';

    const { data } = $props();
    const project = $derived(await useProject());
</script>

<div class="mx-auto w-full max-w-container-lg">
    <h1 class="sr-only">
        {data.namespace.kind === 'user' ? data.namespace.user.name : ''} / {project.name}
    </h1>
    <div>
        <div class="flex items-start justify-between">
            <div>
                <p class="text-title-xs font-ui font-semibold text-fg-emph">
                    {project.name}
                </p>
            </div>
            <div>
                <ProjectActions />
            </div>
        </div>
        {#if project.summary && project.summary.length > 0}
            <p class="max-w-container-md text-fg-dim text-sm mt-2">
                {project.summary}
            </p>
        {/if}
    </div>
    <div class="mt-4 flex flex-col gap-8 lg:flex-row">
        <div class="lg:flex-1">
            <Description />
        </div>
        <div class="lg:min-w-xs">
            <Team />
        </div>
    </div>
</div>
