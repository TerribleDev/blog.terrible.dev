Compress webp

`find . -iname '*.png' -exec  cwebp -lossless '{}' -o '{}'.webp \;
find . -iname '*.jpg' -exec  cwebp '{}' -o '{}'.webp \;`