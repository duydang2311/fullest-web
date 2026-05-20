<script>
    import { TrashOutline } from '~/lib/components/icons';
    import { C } from '~/lib/utils/styles';
    import { deleteTask } from './page.remote';
    import { useTask } from './utils.svelte';

    const task = $derived(await useTask());
</script>

<form {...deleteTask}>
    <input {...deleteTask.fields.id.as('text', task.id)} type="hidden" />
    <button
        type="submit"
        class="{C.button({
            variant: 'negative',
            ghost: true,
            size: 'sm',
        })} flex items-center gap-2 w-full"
    >
        <TrashOutline />
        <span>Delete</span>
    </button>
</form>
