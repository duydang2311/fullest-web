import { crop, PhotonImage, resize, SamplingFilter } from '@cf-wasm/photon';
import { attempt } from '@duydang2311/attempt';
import encodeWebp, { init } from '@jsquash/webp/encode';
import { privateHandler } from '../lib/utils';
import { putUserObject } from './put-user-object';

// @ts-ignore
import WEBP_ENC_WASM from '../../node_modules/@jsquash/webp/codec/enc/webp_enc_simd.wasm';

const PFP_SIZE = 512;

export const putUserPfp = privateHandler(async (req, env, ctx) => {
    let bytes: Uint8Array<ArrayBufferLike> | ArrayBuffer = new Uint8Array(await req.arrayBuffer());
    const image = PhotonImage.new_from_byteslice(bytes);
    if (image.get_width() > PFP_SIZE || image.get_height() > PFP_SIZE) {
        const size = Math.min(image.get_width(), image.get_height());
        const top = (image.get_height() - size) / 2;
        const left = (image.get_width() - size) / 2;
        const cropped = crop(image, left, top, size, size);
        const resized = resize(cropped, PFP_SIZE, PFP_SIZE, SamplingFilter.CatmullRom);
        const encoded = await attempt.async(async () => {
            await init(WEBP_ENC_WASM);
            return await encodeWebp(resized.get_image_data(), { quality: 80 });
        })();
        bytes = encoded.ok ? encoded.data : resized.get_bytes_webp();
        cropped.free();
        resized.free();
    }
    image.free();

    const newRequest = new Request<unknown, IncomingRequestCfProperties>(req, { body: bytes });
    newRequest.headers.set('Content-Type', 'image/webp');
    return await putUserObject(newRequest, env, ctx);
});
