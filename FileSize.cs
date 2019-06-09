using System;

namespace ImageOptimizer
{
    public struct FileSize
    {
        public FileSize(long bytes)
        {
            Bytes = bytes;
        }

        public long Bytes { get; }

        public double KiloBytes => Bytes / 1024;

        public double MegaBytes => KiloBytes / 1024;

        public double GigaBytes => MegaBytes / 1024;

        public static implicit operator FileSize(long bytes)
        {
            return new FileSize(bytes);
        }

        public static implicit operator long(FileSize fileSize)
        {
            return fileSize.Bytes;
        }

        public static implicit operator string(FileSize fileSize)
        {
            return fileSize.ToString();
        }

        public override string ToString()
        {
            if (GigaBytes > 1)
                return $"{GigaBytes:G2} GB";

            if (MegaBytes > 1)
                return $"{MegaBytes:G2} MB";

            if (KiloBytes > 1)
                return $"{KiloBytes:G2} KB";

            return $"{Bytes} B";
        }
    }
}