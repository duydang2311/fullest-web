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

<div class="mx-auto">
    <h1 class="sr-only">
        {data.namespace.kind === 'user' ? data.namespace.user.name : ''} / {project.name}
    </h1>
    <div>
        <div class="mx-auto">
            <div class="flex items-start justify-between">
                <div>
                    <Breadcrumbs />
                    <!-- <Avatar user={data.namespace.user} class="size-avatar-sm shrink-0" /> -->
                    <p class="text-title-xs font-ui font-semibold">
                        <a
                            href="/{page.params.namespace}/{page.params.slug}"
                            class="c-link not-hover:text-fg-emph"
                        >
                            {project.name}
                        </a>
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
    </div>
    <div class="mt-4 flex flex-col gap-y-8 lg:flex-row lg:divide-x lg:divide-surface-border">
        <div class="lg:flex-6 lg:pr-8">
            <div class="max-w-container-lg">
                <Description />
            </div>
        </div>
        <div class="lg:flex-2 lg:pl-8">
            <Team />
        </div>
    </div>
</div>
