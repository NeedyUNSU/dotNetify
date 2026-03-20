import NextImage, { type ImageProps } from 'next/image';

import { getBase64 } from '@/lib/get-base64';

interface Props extends ImageProps {
	src: string;
}

export const BlurredImage = async ({ src, ...rest }: Props) => {
	if (!src) {
		return <NextImage src="/album.jpg" {...rest} />;
	}
	return <NextImage src={src} placeholder="blur" blurDataURL={await getBase64(src)} {...rest} />;
};