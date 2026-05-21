<script lang="ts">
    import { tooltip } from '~/lib/components/builders.svelte';
    import { PlusOutline } from '~/lib/components/icons';
    import { C } from '~/lib/utils/styles';
</script>

<div>
    <div class="flex items-center gap-2 text-fg-muted">
        <h2 class="text-xs tracking-wide">TEAM</h2>
        <div class="ml-auto" {@attach tooltip('Invite people')}>
            <button type="button" class={C.button({ ghost: true, icon: true })}>
                <PlusOutline />
            </button>
        </div>
    </div>
</div>
