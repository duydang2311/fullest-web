<script lang="ts">
	import { enhance } from '$app/forms';
	import { button, field, input, label } from '~/lib/utils/styles';

	// TODO: handle form result
	// const { form } = $props();
</script>

<h1 class="font-bold">Complete Registration</h1>
<p class="text-base-fg-dim">Fill in the required information to create your new account.</p>
<form method="post" use:enhance class="mt-4">
	<fieldset class="space-y-4">
		<div class={field()}>
			<label for="name" class={label()}>Username</label>
			<input type="text" id="name" name="name" placeholder="Enter username" class={input()} />
		</div>
		<button
			type="submit"
			class="{button({
				variant: 'primary',
				filled: true,
				outlined: true,
			})} w-full"
		>
			Sign Up
		</button>
	</fieldset>
</form>
