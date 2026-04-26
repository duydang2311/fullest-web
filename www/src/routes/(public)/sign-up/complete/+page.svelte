<script lang="ts">
    import { enhance } from '$app/forms';
    import LogoType from '~/lib/components/LogoType.svelte';
    import { C } from '~/lib/utils/styles';

    // TODO: handle form result
    // const { form } = $props();
</script>

<main class="flex h-full flex-col justify-center gap-8 p-8">
    <div class="max-w-lg mx-auto w-full">
        <LogoType />
        <h1 class="text-title-md text-fg-emph">Complete Registration</h1>
        <p class="text-sm text-fg-muted mt-2">
            Fill in the required information to create your new account.
        </p>
        <form method="post" use:enhance class="mt-8">
            <fieldset class="space-y-4">
                <div class={C.field()}>
                    <label for="name" class={C.label()}>Username</label>
                    <input
                        type="text"
                        id="name"
                        name="name"
                        placeholder="Enter username"
                        class={C.input()}
                    />
                </div>
                <button
                    type="submit"
                    class="{C.button({
                        variant: 'primary',
                        filled: true,
                    })} w-full"
                >
                    Sign up
                </button>
            </fieldset>
        </form>
    </div>
</main>
