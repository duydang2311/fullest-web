<script lang="ts">
    import { env } from '$env/dynamic/public';
    import { portal } from '@zag-js/svelte';
    import { createFileUpload, createMenu } from '~/lib/components/builders.svelte';
    import { PencilOutline, TrashOutline, UploadOutline } from '~/lib/components/icons';
    import type { User } from '~/lib/models/user';
    import { button } from '~/lib/utils/styles';

    const {
        user,
        onChanged,
    }: {
        user: Pick<User, 'id' | 'name' | 'displayName' | 'imageKey' | 'imageVersion'>;
        onChanged: (data: { key: string; version: number } | null) => void;
    } = $props();
    const id = $props.id();
    const fileUpload = createFileUpload({
        id: 'fileupload--' + id,
        accept: 'image/*',
        maxFileSize: 1024 * 1024 * 4,
        acceptedFiles: undefined,
        onFileAccept: async (details) => {
            const file = details.files[0];
            if (!file) {
                // TODO: set user image to null
                onChanged(null);
                return;
            }
            const uploaded = await fetch('/_/api/users/profile-upload-request', {
                method: 'POST',
            })
                .then((a) => a.json<{ accessToken: string; key: string }>())
                .then((result) =>
                    fetch(`${env.PUBLIC_ASSET_API_ORIGIN}${result.key}`, {
                        method: 'PUT',
                        body: file,
                        headers: {
                            'Content-Type': file.type,
                            Authorization: `Bearer ${result.accessToken}`,
                        },
                    })
                )
                .then((a) => a.json<{ key: string; version: number }>());
            onChanged(uploaded);
        },
    });
    const menu = createMenu({
        id: 'menu--' + id,
        onSelect: (details) => {
            switch (details.value) {
                case 'upload':
                    menu.api.setOpen(false);
                    fileUpload.api.openFilePicker();
                    break;
                case 'delete':
                    if (fileUpload.api.acceptedFiles.length > 0) {
                        fileUpload.api.clearFiles();
                    } else {
                        onChanged(null);
                    }
                    break;
            }
        },
    });
</script>

<div {...fileUpload.api.getRootProps()}>
    <div {...fileUpload.api.getDropzoneProps()} tabindex="-1">
        <input {...fileUpload.api.getHiddenInputProps()} />
        <div class="relative group rounded-full">
            {#if user.imageKey && user.imageVersion}
                <img
                    src="{env.PUBLIC_ASSET_API_ORIGIN}{user.imageKey}?v={user.imageVersion}"
                    alt={user.displayName ?? user.name}
                    class="size-avatar-lg rounded-full border-2 border-base-border"
                />
            {:else}
                <div
                    class="size-avatar-lg rounded-full bg-base-light border-2 border-base-border"
                ></div>
            {/if}
            <button
                type="button"
                title="Upload"
                {...menu.api.getTriggerProps()}
                class="{button({
                    variant: 'base',
                    filled: true,
                    outlined: true,
                })} flex items-center gap-2 absolute bottom-0 left-0 -translate-y-1/2 translate-x-1/4 text-sm"
            >
                <PencilOutline />
                Edit
            </button>
            <div use:portal {...menu.api.getPositionerProps()}>
                <ul
                    {...menu.api.getContentProps()}
                    class="c-menu--content flex flex-col gap-1 text-sm"
                >
                    <li>
                        <button
                            type="button"
                            {...menu.api.getItemProps({ value: 'upload' })}
                            class="c-menu--item flex items-center gap-4 font-medium"
                        >
                            <UploadOutline />
                            Upload
                        </button>
                    </li>
                    {#each [{ value: 'upload', label: 'Upload', icon: UploadOutline }, { value: 'delete', label: 'Delete', icon: TrashOutline }] as item (item.value)}{/each}
                    <li>
                        <button
                            type="button"
                            disabled={user.imageKey == null || user.imageVersion == null}
                            {...menu.api.getItemProps({ value: 'delete' })}
                            class="c-menu--item c-menu--item--negative flex items-center gap-4 font-medium disabled:pointer-events-none disabled:opacity-40"
                        >
                            <TrashOutline />
                            Delete
                        </button>
                    </li>
                </ul>
            </div>
        </div>
    </div>
</div>
