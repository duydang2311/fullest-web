<script lang="ts" module>
    interface Item {
        time: DateTime;
        options?: ToRelativeOptions & { capitalize?: boolean };
        format: string;
    }

    let interval = 0;
    const refs: Ref<Item>[] = [];
    export const add = (ref: Ref<Item>) => {
        refs.push(ref);
        if (interval === 0) {
            interval = setInterval(update, 1000) as unknown as number;
        }
    };
    export const remove = (ref: Ref<Item>) => {
        const index = refs.findIndex((a) => a === ref);
        if (index !== -1) {
            refs.splice(index, 1);
        }
        if (refs.length === 0) {
            clearInterval(interval);
            interval = 0;
        }
    };
    export const update = () => {
        for (const ref of refs) {
            if (ref.current) {
                ref.current = {
                    ...ref.current,
                    format: formatRelativeDateTime(ref.current.time) ?? 'N/A',
                };
            }
        }
    };
</script>

<script lang="ts">
    import { createRef, watch, type Ref } from '@duydang2311/svutils';
    import { DateTime, type ToRelativeOptions } from 'luxon';
    import { onMount, untrack } from 'svelte';
    import { formatRelativeDateTime } from '../utils/date';

    let { time }: { time: string | DateTime } = $props();

    const dt = untrack(() => {
        return typeof time === 'string' ? DateTime.fromISO(time) : time;
    });
    const ref = createRef({
        time: dt,
        format: formatRelativeDateTime(dt) ?? 'N/A',
    });
    const absoluteFormat = $derived(dt.toFormat('yyyy-MM-dd HH:mm'));

    watch.pre(() => time)(() => {
        const normalized = typeof time === 'string' ? DateTime.fromISO(time) : time;
        ref.current = {
            time: normalized,
            format: formatRelativeDateTime(normalized) ?? 'N/A',
        };
    });

    onMount(() => {
        add(ref);
        return () => {
            remove(ref);
        };
    });
</script>

{#if ref.current}
    <time datetime={absoluteFormat} title={absoluteFormat}>
        {ref.current.format}
    </time>
{/if}
