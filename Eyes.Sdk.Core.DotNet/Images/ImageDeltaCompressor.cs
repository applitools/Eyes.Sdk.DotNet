namespace Applitools.Utils.Images
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using System.IO.Compression;

    /// <summary>
    /// Compresses a target image by saving its differences from a source image.
    /// </summary>
    /// Compressed image streams have the following format:
    /// [0-9] applitools
    /// [10] compression format
    /// [11-12] source image id length (in bytes)
    /// [13-] id (UTF8 encoded)
    /// [] format specific encoding.
    public static class ImageDeltaCompressor
    {
        #region Fields

        private const string EndOfStreamErr_ = "Stream truncated! ({0})";
        private const string InvalidFormatErr_ = "Invalid stream format ({0})";
        private const byte FormatSource_ = 0;
        private const byte FormatTarget_ = 1;
        private const byte FormatXorPng_ = 2;
        private const byte FormatRawBlocks_ = 3;

        private static readonly byte[] Preamble_ = Encoding.ASCII.GetBytes("applitools");

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Compresses the target image by saving its differences from the source image and 
        /// write the result to the input stream.
        /// If the source image is <c>null</c> or not of the same size as the target image, the 
        /// target image written to the stream in PNG format.
        /// </summary>
        public static unsafe void CompressByRawBlocks(
            Bitmap target,
            byte[] targetEncoded,
            Bitmap source,
            string sourceId,
            Stream stream,
            int blockSize = 10)
        {
            ArgumentGuard.NotNull(target, nameof(target));
            ArgumentGuard.NotNull(targetEncoded, nameof(targetEncoded));
            ArgumentGuard.NotNull(stream, nameof(stream));

            try
            {
                Size? sourceSize = source?.Size;
                if (sourceSize == null || sourceSize != target.Size)
                {
                    stream.Write(targetEncoded, 0, targetEncoded.Length);
                    return;
                }
            }
            catch (ArgumentException)
            {
                stream.Write(targetEncoded, 0, targetEncoded.Length);
                return;
            }

            var columns = (target.Width / blockSize) + ((target.Width % blockSize) == 0 ? 0 : 1);
            var rows = (target.Height / blockSize) + ((target.Height % blockSize) == 0 ? 0 : 1);

            WriteHeader(FormatRawBlocks_, sourceId ?? string.Empty, stream);
            CommonUtils.ToBytesBE((short)blockSize, stream);

            BitmapData sbd = null;
            BitmapData tbd = null;
            try
            {
                var rect = new Rectangle(Point.Empty, source.Size);
                sbd = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                tbd = target.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                var channelBytes = new byte[blockSize * blockSize];
                using (var compressed = new DeflateStream(stream, CompressionLevel.Optimal, true))
                {
                    fixed (byte* channelBytesPtr = channelBytes)
                    {
                        for (byte channel = 0; channel < 3; ++channel)
                        {
                            var blockNumber = 0;
                            for (int row = 0; row < rows; ++row)
                            {
                                for (int column = 0; column < columns; ++column)
                                {
                                    if (!CompareAndCopyBlockChannelData(
                                        (byte*)sbd.Scan0,
                                        (byte*)tbd.Scan0,
                                        source.Size,
                                        sbd.Stride,
                                        blockSize,
                                        column,
                                        row,
                                        channel,
                                        channelBytesPtr,
                                        out int count))
                                    {
                                        compressed.WriteByte(channel);
                                        CommonUtils.ToBytesBE(blockNumber, compressed);
                                        compressed.Write(channelBytes, 0, count);

                                        if (stream.Position > targetEncoded.Length)
                                        {
                                            stream.Position = 0;
                                            stream.SetLength(targetEncoded.Length);
                                            stream.Write(targetEncoded, 0, targetEncoded.Length);
                                            return;
                                        }
                                    }

                                    ++blockNumber;
                                }
                            }
                        }
                    }

                    compressed.Flush();
                }

                if (stream.Position > targetEncoded.Length)
                {
                    stream.Position = 0;
                    stream.SetLength(targetEncoded.Length);
                    stream.Write(targetEncoded, 0, targetEncoded.Length);
                }
            }
            finally
            {
                if (sbd != null)
                {
                    source.UnlockBits(sbd);
                }

                if (tbd != null)
                {
                    target.UnlockBits(tbd);
                }
            }
        }

        /// <summary>
        /// Writes a compression header to the input stream.
        /// </summary>
        public static void WriteHeader(
            byte format, string sourceId, [ValidatedNotNull] Stream stream)
        {
            ArgumentGuard.NotNull(sourceId, nameof(sourceId));
            ArgumentGuard.NotNull(stream, nameof(stream));

            stream.Write(Preamble_, 0, Preamble_.Length);
            stream.WriteByte(format);
            WriteId_(sourceId, stream);
        }

        /// <summary>
        /// Computes the width and height of the image data contained in the block at the input
        /// column and row.
        /// </summary>
        /// <param name="imageSize">Image size</param>
        /// <param name="blockSize">Block size</param>
        /// <param name="column">Block column index</param>
        /// <param name="row">Block row index</param>
        /// <param name="width">Number of block columns containing image data</param>
        /// <param name="height">Number of block rows containing image data</param>
        public static void GetBlockSize(
            Size imageSize, int blockSize, int column, int row, out int width, out int height)
        {
            width = Math.Min(imageSize.Width - (column * blockSize), blockSize);
            height = Math.Min(imageSize.Height - (row * blockSize), blockSize);
        }

        /// <summary>
        /// Copies the channel content of the specified block from the target image and 
        /// indicates whether it differs from the source image.
        /// </summary>
        /// <param name="sourceImage">Source image RGB buffer</param>
        /// <param name="targetImage">Target image RGB buffer</param>
        /// <param name="imageSize">The size of the source and target image</param>
        /// <param name="stride">The stride of the image</param>
        /// <param name="blockSize">The block size</param>
        /// <param name="blockColumn">The block column index</param>
        /// <param name="blockRow">The block row index</param>
        /// <param name="channel">The channel to compare and copy</param>
        /// <param name="copy">The buffer to copy channel data to (at most <c>bs^2</c>)</param>
        /// <param name="count">The number of bytes copied</param>
        /// <returns>Whether channel data is identical in the two input images</returns>
        public static unsafe bool CompareAndCopyBlockChannelData(
            byte* sourceImage,
            byte* targetImage,
            Size imageSize,
            int stride,
            int blockSize,
            int blockColumn,
            int blockRow,
            byte channel,
            byte* copy,
            out int count)
        {
            bool equals = true;
            GetBlockSize(imageSize, blockSize, blockColumn, blockRow, out int bw, out int bh);

            for (int i = 0; i < bh; ++i)
            {
                var offset = (((blockSize * blockRow) + i) * stride) +
                    (blockSize * blockColumn * 3) + channel;
                var s0 = sourceImage + offset;
                var t0 = targetImage + offset;
                for (int j = 0; j < bw; ++j)
                {
                    if (*s0 != *t0)
                    {
                        equals = false;
                    }

                    *(copy++) = *t0;
                    s0 += 3;
                    t0 += 3;
                }
            }

            count = bw * bh;
            return equals;
        }

        #endregion

        #region Private

        private static void WriteId_(string id, Stream stream)
        {
            var bytes = Encoding.UTF8.GetBytes(id);
            CommonUtils.ToBytesBE((short)bytes.Length, stream);
            stream.Write(bytes, 0, bytes.Length);
        }

        #endregion

        #endregion
    }
}
