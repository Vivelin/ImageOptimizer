using System;

namespace ImageOptimizer
{
    public class PhotoOptimizerOptions
    {
        public static readonly PhotoOptimizerOptions Default
            = new PhotoOptimizerOptions();

        private const int KB = 1024;

        private const int MB = 1024 * KB;

        public int MinQuality { get; set; } = 80;
        public int QualityStep { get; set; } = 5;
        public int[] Sizes { get; set; } = new[] { int.MaxValue, 3200, 2800, 2400, 2000, 1600, 1200, 900, 600 };
        public long TargetSize { get; set; } = 8 * MB;
    }
}