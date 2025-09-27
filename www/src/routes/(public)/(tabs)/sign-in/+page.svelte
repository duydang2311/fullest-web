<script lang="ts">
	import { Google } from '~/lib/components/icons';
	import { button } from '~/lib/utils/styles';
</script>

<div class="flex h-full flex-col justify-between gap-8">
	<form>
		<fieldset class="space-y-4">
			<div>
				<label for="email" class="text-base-fg-dim block">Email address</label>
				<input
					id="email"
					name="email"
					type="email"
					placeholder="Enter email address"
					class="bg-base-light border-base-border focus-visible:ring-focus-primary placeholder:text-base-fg-muted w-full rounded-xl border px-3 py-2 transition focus-visible:outline-none"
				/>
			</div>
			<div>
				<label for="password" class="text-base-fg-dim block">Password</label>
				<input
					id="password"
					name="password"
					type="password"
					placeholder="Enter password"
					class="bg-base-light border-base-border placeholder:text-base-fg-muted focus-visible:ring-focus-primary w-full rounded-xl border px-3 py-2 transition focus-visible:outline-none"
				/>
			</div>
			<button
				type="submit"
				class="{button({
					variant: 'primary',
					filled: true,
					outlined: true,
				})} w-full capitalize"
			>
				Sign in
			</button>
		</fieldset>
	</form>
	<div>
		<div class="mb-4 flex items-center gap-4">
			<div class="bg-base-border h-px flex-1"></div>
			<span class="text-base-fg-muted uppercase">or</span>
			<div class="bg-base-border h-px flex-1"></div>
		</div>
		<a
			href="/api/externals/auth/google/request"
			class="{button({
				variant: 'base',
				filled: true,
				outlined: true,
			})} flex w-full items-center justify-center gap-2"
			tabindex="0"
		>
			<Google />
			Sign in with Google
		</a>
	</div>
</div>
