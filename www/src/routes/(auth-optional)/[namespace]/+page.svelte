<script lang="ts">
    import UserPage from './UserPage.svelte';

    const { data } = $props();
</script>

{#if data.namespace.kind === 'user'}
    <UserPage {data} />
{/if}
