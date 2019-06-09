# ImageOptimizer
A proof of concept tool for optimizing images for a specific target size.

This project uses [SkiaSharp] which by itself already provides some optimization (10-14% in images I’ve tested with). The program gradually reduces JPEG quality and image dimensions until a set target size is reached (e.g. 8 MB for Discord’s regular upload limit).

[SkiaSharp]: https://github.com/mono/SkiaSharp
