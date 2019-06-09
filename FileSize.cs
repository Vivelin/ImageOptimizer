using System;

namespace ImageOptimizer
{
    /// <summary>
    /// Represents the file size of an amount of data.
    /// </summary>
    public struct FileSize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSize"/> structure
        /// with the specified amount of bytes.
        /// </summary>
        /// <param name="bytes">
        /// The number of bytes to represent as a file size.
        /// </param>
        public FileSize(long bytes)
        {
            Bytes = bytes;
        }

        /// <summary>
        /// Gets the size as a number of bytes.
        /// </summary>
        public long Bytes { get; }

        /// <summary>
        /// Gets the size in kilobytes.
        /// </summary>
        public double KiloBytes => Bytes / 1024;

        /// <summary>
        /// Get the size in megabytes.
        /// </summary>
        public double MegaBytes => KiloBytes / 1024;

        /// <summary>
        /// Get the size in gigabytes.
        /// </summary>
        public double GigaBytes => MegaBytes / 1024;

        /// <summary>
        /// Converts the number of bytes to a new <see cref="FileSize"/>.
        /// </summary>
        /// <param name="bytes">The number of bytes to represent.</param>
        public static implicit operator FileSize(long bytes)
        {
            return new FileSize(bytes);
        }

        /// <summary>
        /// Converts the <see cref="FileSize"/> to the underlying number of
        /// bytes.
        /// </summary>
        /// <param name="fileSize">The <see cref="FileSize"/> to convert.</param>
        public static implicit operator long(FileSize fileSize)
        {
            return fileSize.Bytes;
        }

        /// <summary>
        /// Converts the <see cref="FileSize"/> to a string representation.
        /// </summary>
        /// <param name="fileSize">The <see cref="FileSize"/> to convert.</param>
        public static implicit operator string(FileSize fileSize)
        {
            return fileSize.ToString();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="FileSize"/> in an
        /// automatically determined unit.
        /// </summary>
        /// <returns>A string representing this <see cref="FileSize"/>.</returns>
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