<script lang="ts">
    import { EyeOutline, StarOutline } from '~/lib/components/icons';
    import { button } from '~/lib/utils/styles';
</script>

<div class="flex gap-4">
    <button
        type="button"
        class="{button({
            variant: 'base',
            ghost: true,
            outlined: true,
        })} flex p-0 text-sm tracking-tight"
    >
        <div class="px-2 py-1 flex items-center gap-1">
            <EyeOutline />
            <span>Watch</span>
        </div>
        <div
            class="text-base-fg border-l border-l-base-border px-2 py-1 rounded-r-lg content-center"
        >
            0
        </div>
    </button>
    <button
        type="button"
        class="{button({
            variant: 'base',
            ghost: true,
            outlined: true,
        })} flex p-0 text-sm"
    >
        <div class="px-2 py-1 flex items-center gap-1">
            <StarOutline />
            <span>Star</span>
        </div>
        <div
            class="text-base-fg border-l border-l-base-border px-2 py-1 rounded-r-lg content-center"
        >
            0
        </div>
    </button>
</div>
