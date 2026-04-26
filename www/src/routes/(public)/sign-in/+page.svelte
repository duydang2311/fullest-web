<script lang="ts">
    import { GitHub, Google } from '~/lib/components/icons';
    import LogoType from '~/lib/components/LogoType.svelte';
    import { C } from '~/lib/utils/styles';
</script>

<main class="flex h-full flex-col justify-center gap-8 p-8">
    <div class="max-w-lg mx-auto w-full">
        <LogoType />
        <h1 class="text-title-md text-fg-emph">Log in to Fullest</h1>
        <p class="text-sm font-medium text-fg-muted mt-8">Connect to Fullest with:</p>
        <div class="flex gap-2 flex-wrap *:basis-48 *:flex-1 mt-2">
            <a
                href="/auth/google/start"
                class="{C.button({
                    variant: 'base',
                    ghost: true,
                    outlined: true,
                })} flex items-center justify-center gap-2"
            >
                <Google />
                Google
            </a>
            <a
                href="#"
                class="{C.button({
                    variant: 'base',
                    ghost: true,
                    outlined: true,
                })} flex items-center justify-center gap-2"
                aria-disabled="true"
            >
                <GitHub />
                GitHub
            </a>
        </div>

        <div class="mt-16 flex items-center gap-4">
            <div class="bg-surface-border h-px flex-1"></div>
            <span class="text-fg-muted text-xs">Or continue with</span>
            <div class="bg-surface-border h-px flex-1"></div>
        </div>
        <form class="mt-4">
            <fieldset class="space-y-4">
                <div class={C.field()}>
                    <label for="email" class={C.label()}>Email address</label>
                    <input
                        id="email"
                        name="email"
                        type="email"
                        placeholder="Enter email address"
                        disabled
                        class={C.input()}
                    />
                </div>
                <div class={C.field()}>
                    <label for="password" class={C.label()}>Password</label>
                    <input
                        id="password"
                        name="password"
                        type="password"
                        placeholder="Enter password"
                        disabled
                        class={C.input()}
                    />
                </div>
                <button
                    type="submit"
                    class="{C.button({
                        variant: 'primary',
                        filled: true,
                    })} w-full"
                    disabled
                >
                    Sign in
                </button>
            </fieldset>
        </form>
    </div>
</main>
