## Compress webp

find . -iname '*.png' -exec  cwebp -lossless '{}' -o '{}'.webp \;
find . -iname '*.jpg' -exec  cwebp '{}' -o '{}'.webp \;
find . -iname '*.gif' -exec  gif2webp -mixed '{}' -o '{}'.webp \;


## resize image

find . -iname '*' -exec  convert '{}' -resize 750 '{}' \;