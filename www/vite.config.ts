import { sveltekit } from '@sveltejs/kit/vite';
import tailwindcss from '@tailwindcss/vite';
import { FileSystemIconLoader } from 'unplugin-icons/loaders';
import Icons from 'unplugin-icons/vite';
import { defineConfig } from 'vite';
import devtoolsJson from 'vite-plugin-devtools-json';
import { guardPlugin } from './src/lib/plugins/vite-guard';

export default defineConfig({
    plugins: [
        tailwindcss(),
        sveltekit(),
        devtoolsJson(),
        Icons({
            compiler: 'svelte',
            customCollections: {
                custom: FileSystemIconLoader('./src/lib/assets/icons', (svg) => svg),
            },
        }),
        guardPlugin(),
    ],
});
