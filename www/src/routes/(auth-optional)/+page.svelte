<script lang="ts">
    import { untrack } from 'svelte';
    import { setPageContext } from './__page__/utils.svelte';
    import Anonymous from './Anonymous.svelte';

    const { data } = $props();

    untrack(() => {
        if (data.session) {
            setPageContext({
                projectList: data.projectList,
            });
        }
    });
</script>

{#if data.session}
    <data.Authenticated />
{:else}
    <Anonymous />
{/if}
