<script lang="ts">
    const { label, content }: { label: string; content: string } = $props();
</script>

<div class="flex justify-between gap-4 items-center px-2 py-1 text-sm text-fg-muted">
    <span>
        {label}
    </span>
    <span>
        {content}
    </span>
</div>
