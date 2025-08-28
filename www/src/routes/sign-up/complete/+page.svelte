<script lang="ts">
	import { enhance } from '$app/forms';

	const { form } = $props();
</script>

{#if form}
	{JSON.stringify(form)}
{/if}
<form method="post" use:enhance>
	<input type="text" id="name" name="name" placeholder="Enter username" />
	<button type="submit" class="capitalize">Sign up</button>
</form>
