import { env } from '$env/dynamic/public';

export function buildImageUrl(key: string, version: number) {
    return `${env.PUBLIC_ASSET_API_ORIGIN}${key}?v=${version}`;
}
