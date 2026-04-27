import { sentrySvelteKit } from '@sentry/sveltekit';
import { sveltekit } from '@sveltejs/kit/vite';
import tailwindcss from '@tailwindcss/vite';
import { FileSystemIconLoader } from 'unplugin-icons/loaders';
import Icons from 'unplugin-icons/vite';
import { defineConfig, loadEnv } from 'vite';
import devtoolsJson from 'vite-plugin-devtools-json';
import { guardPlugin } from './src/lib/plugins/vite-guard';

export default defineConfig((e) => {
    const env = loadEnv(e.mode, process.cwd(), '');
    return {
        plugins: [
            sentrySvelteKit({
                org: 'duy-dang',
                project: 'fullest-web',
                authToken: process.env.SENTRY_AUTH_TOKEN || env.SENTRY_AUTH_TOKEN,
            }),
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
        build: {
            sourcemap: 'hidden',
        },
    };
});
