<script lang="ts">
    import { createRef } from '@duydang2311/svutils';
    import AuthenticatedHeader from '~/lib/components/AuthenticatedHeader.svelte';
    import Footer from '~/lib/components/Footer.svelte';
    import type { PageData } from './$types';
    import UserProfileImage from './UserProfileImage.svelte';

    const {
        data,
    }: {
        data: PageData;
    } = $props();
    const userRef = createRef(() => data.namespace.user);
</script>

<div class="min-h-screen flex flex-col">
    {#if data.session}
        <AuthenticatedHeader />
    {:else}
        <header></header>
    {/if}
    <main class="flex-1 px-8 py-4">
        <h1 class="sr-only">{userRef.current.name}'s profile</h1>
        <div class="max-w-container-xl full w-full mx-auto">
            <div class="w-fit">
                <UserProfileImage
                    user={userRef.current}
                    onChanged={(data) => {
                        if (!data) {
                            userRef.current = {
                                ...userRef.current,
                                imageKey: undefined,
                                imageVersion: undefined,
                            };
                        } else {
                            userRef.current = {
                                ...userRef.current,
                                imageKey: data.key,
                                imageVersion: data.version,
                            };
                        }
                    }}
                />
                <p class="text-title-sm font-bold mt-2">{userRef.current.name}</p>
            </div>
        </div>
    </main>
    <Footer />
</div>
