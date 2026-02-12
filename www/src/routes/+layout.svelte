<script lang="ts">
    import '@fontsource-variable/geist-mono';
    import '@fontsource-variable/inter';
    import '../app.css';

    import favicon from '$lib/assets/favicon.svg';

    const { children } = $props();
</script>

<svelte:head>
    <link rel="icon" href={favicon} />
</svelte:head>

{@render children?.()}
