using System;
using System.IO;

using Decos.Diagnostics;

using SkiaSharp;

namespace ImageOptimizer
{
    public class PhotoOptimizer
    {
        public PhotoOptimizer(ILog<PhotoOptimizer> log)
            : this(log, PhotoOptimizerOptions.Default)
        {
        }

        public PhotoOptimizer(ILog<PhotoOptimizer> log, PhotoOptimizerOptions options)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            Options = options;
            Log = log;
        }

        public PhotoOptimizerOptions Options { get; }

        protected ILog<PhotoOptimizer> Log { get; }

        public (int, int) Optimize(string sourcePath, string destinationPath)
        {
            using (var source = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var destination = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                return Optimize(source, destination);
            }
        }

        public (int, int) Optimize(Stream source, Stream destination)
        {
            var originalSize = source.Length;
            using (var sourceBitmap = LoadBitmap(source))
            {
                foreach (var maxSize in Options.Sizes)
                {
                    using (var resized = Resize(sourceBitmap, maxSize))
                    {
                        Log.Debug($"Size: {resized.Width}×{resized.Height}");

                        for (var quality = 100; quality >= Options.MinQuality; quality -= Options.QualityStep)
                        {
                            Log.Debug($"Quality: {quality}");
                            using (var encoded = Encode(resized, quality))
                            {
                                if (encoded.Size < Options.TargetSize)
                                {
                                    var sizeDifference = originalSize - encoded.Size;
                                    var sizeReduction = sizeDifference / (double)originalSize;
                                    Log.Info("Target size reached.", new
                                    {
                                        OptimizedSize = new FileSize(encoded.Size),
                                        OriginalSize = new FileSize(originalSize),
                                        SizeDifference = new FileSize(sizeDifference),
                                        SizeReduction = sizeReduction.ToString("P0"),
                                        FinalQuality = quality,
                                        FinalSize = maxSize == int.MaxValue ? (int?)null : maxSize
                                    });
                                    encoded.SaveTo(destination);
                                    return (maxSize, quality);
                                }
                            }
                        }
                    }
                }

                throw new InvalidOperationException("The source image cannot be optimized for the current options.");
            }
        }

        private SKData Encode(SKBitmap source, int quality)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException(nameof(quality), "Value must be in the range [0, 100].");

            using (var image = SKImage.FromBitmap(source))
            {
                return image.Encode(SKEncodedImageFormat.Jpeg, quality);
            }
        }

        private (int, int) GetTargetDimensions(int width, int height, int maxSize)
        {
            if (width > height)
            {
                var factor = (double)height / width;
                return (maxSize, (int)Math.Ceiling(height * factor));
            }
            else
            {
                var factor = (double)width / height;
                return ((int)Math.Ceiling(width * factor), maxSize);
            }
        }

        private SKBitmap LoadBitmap(Stream sourceStream)
        {
            using (var codec = SKCodec.Create(sourceStream))
            {
                var bitmap = SKBitmap.Decode(codec);
                Log.Debug($"Orientation: {codec.EncodedOrigin}");

                SKBitmap rotated;
                switch (codec.EncodedOrigin)
                {
                    case SKEncodedOrigin.BottomRight:
                        using (var surface = new SKCanvas(bitmap))
                        {
                            surface.RotateDegrees(180, bitmap.Width / 2f, bitmap.Height / 2f);
                            surface.DrawBitmap(bitmap.Copy(), 0, 0);
                        }

                        return bitmap;

                    case SKEncodedOrigin.RightTop:
                        rotated = new SKBitmap(bitmap.Height, bitmap.Width);

                        using (var surface = new SKCanvas(rotated))
                        {
                            surface.Translate(rotated.Width, 0);
                            surface.RotateDegrees(90);
                            surface.DrawBitmap(bitmap, 0, 0);
                        }

                        return rotated;

                    case SKEncodedOrigin.LeftBottom:
                        rotated = new SKBitmap(bitmap.Height, bitmap.Width);

                        using (var surface = new SKCanvas(rotated))
                        {
                            surface.Translate(0, rotated.Height);
                            surface.RotateDegrees(270);
                            surface.DrawBitmap(bitmap, 0, 0);
                        }

                        return rotated;

                    default:
                        return bitmap;
                }
            }
        }

        private SKBitmap Resize(SKBitmap source, int maxSize)
        {
            if (source.Width < maxSize && source.Height < maxSize)
                return source;

            var (width, height) = GetTargetDimensions(source.Width, source.Height, maxSize);
            return source.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
        }
    }
}