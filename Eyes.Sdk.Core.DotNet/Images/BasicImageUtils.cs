namespace Applitools.Utils.Images
{
    using System;
    using System.Drawing;
    //using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Security.Cryptography;

    /// <summary>
    /// Basic image utilities.
    /// </summary>
    public static class BasicImageUtils
    {
        #region Methods

        #region Streams

        /// <summary>
        /// Gets in-memory copy of the input image in the specified format as a stream.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Stream should not be disposed")]
        public static MemoryStream GetStream(this Image image, ImageFormat format = null)
        {
            format = format ?? ImageFormat.Png;

            ArgumentGuard.NotNull(image, nameof(image));

            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        #endregion

        public static Bitmap ScaleImage(Bitmap image, IScaleProvider scaleProvider)
        {
            return ScaleImage(image, scaleProvider.ScaleRatio);
        }

        public static Bitmap ScaleImage(Bitmap image, double scaleRatio)
        {
            ArgumentGuard.NotNull(image, nameof(image));
            ArgumentGuard.GreaterOrEqual(scaleRatio, 0, nameof(scaleRatio));

            int targetWidth = (int)Math.Ceiling(image.Width * scaleRatio);
            int targetHeight = (int)Math.Ceiling(image.Height * scaleRatio);

            if (targetWidth == image.Width && targetHeight == image.Height)
            {
                return image;
            }

            #region best implementation but based on GDI+ internal implementation.
            //Bitmap resizedImage = new Bitmap(targetWidth, targetHeight, PixelFormat.Format24bppRgb);
            //resizedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            //using (var graphics = Graphics.FromImage(resizedImage))
            //using (var wrapMode = new ImageAttributes())
            //{
            //    graphics.CompositingMode = CompositingMode.SourceCopy;
            //    graphics.CompositingQuality = CompositingQuality.HighQuality;
            //    graphics.InterpolationMode = InterpolationMode.Bicubic;
            //    graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //    graphics.PixelOffsetMode = PixelOffsetMode.Half;
            //    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            //    graphics.DrawImage(image, new Rectangle(0, 0, targetWidth, targetHeight), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            //}
            #endregion

            #region I don't remember why we try to scale down incrementally, or what's wrong with just scaling down.
            Bitmap resizedImage = ScaleImageBicubic_((Bitmap)image, targetWidth, targetHeight);
            #endregion

            #region current implementation
            //Bitmap resizedImage;
            //if (targetWidth > image.Width || targetHeight > image.Height)
            //{
            //    resizedImage = ScaleImageBicubic_((Bitmap)image, targetWidth, targetHeight);
            //}
            //else
            //{
            //    resizedImage = ScaleImageIncrementally_((Bitmap)image, targetWidth, targetHeight);
            //}
            #endregion

            return resizedImage;
        }

        #region Private

        private static byte InterpolateCubic_(int x0, int x1, int x2, int x3, double t)
        {
            int a0 = x3 - x2 - x0 + x1;
            int a1 = x0 - x1 - a0;
            int a2 = x2 - x0;
            return (byte)Math.Max(0, Math.Min(255, (a0 * (t * t * t)) + (a1 * (t * t)) + (a2 * t) + (x1)));
        }

        private static Bitmap ScaleImageBicubic_(Bitmap srcImage, int targetWidth, int targetHeight)
        {
            Rectangle srcRect = new Rectangle(0, 0, srcImage.Width, srcImage.Height);
            Rectangle destRect = new Rectangle(0, 0, targetWidth, targetHeight);
            BitmapData srcData = srcImage.LockBits(srcRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int wSrc = srcImage.Width;
            int hSrc = srcImage.Height;

            int wM = (int)Math.Max(1, Math.Floor(wSrc / (double)targetWidth));
            int wDst2 = targetWidth * wM;
            int hM = (int)Math.Max(1, Math.Floor(hSrc / (double)targetHeight));
            int hDst2 = targetHeight * hM;

            double wRatio = (double)wSrc / wDst2;// The '(wSrc - 1)' is wrong. it should be 'wSrc', but then it causes array index overflow.
            double hRatio = (double)hSrc / hDst2;// The '(hSrc - 1)' is wrong. it should be 'hSrc', but then it causes array index overflow.

            int i, j, k, xPos, yPos, kPos, buf1Pos, buf2Pos, srcPos;
            double x, y, t;

            Bitmap dstImage = new Bitmap(targetWidth, targetHeight, srcImage.PixelFormat);
            BitmapData destData = dstImage.LockBits(destRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                unchecked
                {
                    // Pass 1 - interpolate rows
                    // buf1 has width of dst2 and height of src
                    byte[] buf1 = new byte[wDst2 * hSrc * 4];
                    for (i = 0; i < hSrc; i++)
                    {
                        byte* strideSrc = (byte*)srcData.Scan0 + srcData.Stride * i;
                        buf1Pos = i * wDst2 * 4;
                        // handle column in index 0 (zero)
                        for (k = 0; k < 4; k++)
                        {
                            int x0 = strideSrc[k];
                            int x1 = strideSrc[k];
                            int x2 = strideSrc[k + 4]; // x + 1
                            int x3 = strideSrc[k + 8]; // x + 2
                            buf1[buf1Pos + k] = InterpolateCubic_(x0, x1, x2, x3, 0);
                        }
                        
                        // handle most columns (indices 1 to n-2)
                        for (j = 1; j < wDst2 - 2; j++)
                        {
                            x = j * wRatio;
                            xPos = (int)Math.Floor(x);
                            t = x - xPos;
                            srcPos = (i * wSrc + xPos) << 2;
                            buf1Pos = (i * wDst2 + j) * 4;
                            for (k = 0; k < 4; k++)
                            {
                                kPos = (xPos << 2) + k;
                                int x0 = strideSrc[kPos - 4]; // x - 1
                                int x1 = strideSrc[kPos];     // x
                                int x2 = strideSrc[kPos + 4]; // x + 1
                                int x3 = strideSrc[kPos + 8]; // x + 2
                                buf1[buf1Pos + k] = InterpolateCubic_(x0, x1, x2, x3, t);
                            }
                        }

                        // handle column in index n-2
                        buf1Pos = (i * wDst2 + (wDst2 - 2)) * 4;
                        x = (wDst2 - 2) * wRatio;
                        xPos = (int)Math.Floor(x);
                        t = x - xPos;
                        for (k = 0; k < 4; k++)
                        {

                            kPos = (xPos << 2) + k;
                            int x0 = strideSrc[kPos - 4];
                            int x1 = strideSrc[kPos];
                            int x2 = strideSrc[kPos + 4];
                            int x3 = (strideSrc[kPos + 4] << 2) - x1;
                            buf1[buf1Pos + k] = InterpolateCubic_(x0, x1, x2, x3, t);
                        }

                        // handle column in index n-1 (last one)
                        buf1Pos = (i * wDst2 + (wDst2 - 1)) * 4;
                        x = (wDst2 - 1) * wRatio;
                        xPos = (int)Math.Floor(x);
                        t = x - xPos;
                        for (k = 0; k < 4; k++)
                        {
                            kPos = (xPos << 2) + k;
                            int x0 = strideSrc[kPos - 4];
                            int x1 = strideSrc[kPos];
                            int x2 = (strideSrc[kPos] << 2) - x1;
                            int x3 = (strideSrc[kPos] << 2) - x1;
                            buf1[buf1Pos + k] = InterpolateCubic_(x0, x1, x2, x3, t);
                        }
                    }

                    // Pass 2 - interpolate columns
                    // buf2 has width and height of dst2
                    byte[] buf2 = new byte[wDst2 * (hDst2 << 2)];
                    byte* bufDst = (byte*)destData.Scan0.ToPointer();
                    for (j = 0; j < wDst2; j++)
                    {
                        buf1Pos = j << 2;
                        buf2Pos = j << 2;
                        for (k = 0; k < 4; k++)
                        {
                            kPos = buf1Pos + k;
                            int y0 = buf1[kPos];                // y
                            int y1 = buf1[kPos];                // y
                            int y2 = buf1[kPos + (wDst2 << 2)]; // y + 1
                            int y3 = buf1[kPos + (wDst2 << 3)]; // y + 2
                            buf2[buf2Pos + k] = InterpolateCubic_(y0, y1, y2, y3, 0);
                        }
                    }
                    for (i = 1; i < hDst2 - 2; i++)
                    {
                        buf1Pos = 0;
                        buf2Pos = 0;
                        y = i * hRatio;
                        yPos = (int)Math.Floor(y);
                        t = y - yPos;
                        for (j = 0; j < wDst2; j++)
                        {
                            buf1Pos = (yPos * wDst2 + j) << 2;
                            buf2Pos = (i * wDst2 + j) << 2;
                            for (k = 0; k < 4; k++)
                            {
                                kPos = buf1Pos + k;
                                int y0 = buf1[kPos - (wDst2 << 2)]; // y - 1
                                int y1 = buf1[kPos];                // y
                                int y2 = buf1[kPos + (wDst2 << 2)]; // y + 1
                                int y3 = buf1[kPos + (wDst2 << 3)]; // y + 2
                                buf2[buf2Pos + k] = InterpolateCubic_(y0, y1, y2, y3, t);
                            }
                        }
                    }
                    y = (hDst2 - 2) * hRatio;
                    yPos = (int)Math.Floor(y);
                    for (j = 0; j < wDst2; j++)
                    {
                        buf1Pos = (yPos * wDst2 + j) << 2;
                        buf2Pos = ((hDst2 - 2) * wDst2 + j) << 2;
                        for (k = 0; k < 4; k++)
                        {
                            kPos = buf1Pos + k;
                            int y0 = buf1[kPos - (wDst2 << 2)]; // y - 1
                            int y1 = buf1[kPos];                // y
                            int y2 = buf1[kPos + (wDst2 << 2)]; // y + 1
                            int y3 = buf1[kPos + (wDst2 << 2)]; // y + 1
                            buf2[buf2Pos + k] = InterpolateCubic_(y0, y1, y2, y3, 0);
                        }
                    }
                    y = (hDst2 - 1) * hRatio;
                    yPos = (int)Math.Floor(y);
                    for (j = 0; j < wDst2; j++)
                    {
                        buf1Pos = (yPos * wDst2 + j) << 2;
                        buf2Pos = ((hDst2 - 1) * wDst2 + j) << 2;
                        for (k = 0; k < 4; k++)
                        {
                            kPos = buf1Pos + k;
                            int y0 = buf1[kPos - (wDst2 << 2)];
                            int y1 = buf1[kPos];
                            int y2 = (buf1[kPos] << 1) - y1;
                            int y3 = (buf1[kPos] << 2) - y1;
                            buf2[buf2Pos + k] = InterpolateCubic_(y0, y1, y2, y3, 0);
                        }
                    }
                    // Pass 3 - scale to dst
                    double m = wM * hM;
                    if (m > 1)
                    {
                        for (i = 0; i < targetHeight; i++)
                        {
                            byte* strideDest = (byte*)destData.Scan0 + destData.Stride * i;
                            for (j = 0; j < targetWidth; j++)
                            {
                                int r = 0;
                                int g = 0;
                                int b = 0;
                                int a = 0;
                                for (y = 0; y < hM; y++)
                                {
                                    yPos = (int)(i * hM + y);
                                    for (x = 0; x < wM; x++)
                                    {
                                        xPos = (int)(j * wM + x);
                                        int xyPos = (yPos * wDst2 + xPos) << 2;
                                        r += buf2[xyPos];
                                        g += buf2[xyPos + 1];
                                        b += buf2[xyPos + 2];
                                        a += buf2[xyPos + 3];
                                    }
                                }

                                int pos = j << 2;
                                strideDest[pos] = (byte)Math.Round(r / m);
                                strideDest[pos + 1] = (byte)Math.Round(g / m);
                                strideDest[pos + 2] = (byte)Math.Round(b / m);
                                strideDest[pos + 3] = (byte)Math.Round(a / m);
                            }
                        }
                    }
                    else
                    {
                        for (i = 0; i < targetHeight; i++)
                        {
                            for (j = 0; j < destData.Stride; j++)
                            {
                                int pos = i * destData.Stride + j;
                                bufDst[pos] = buf2[pos];
                            }
                        }
                    }
                }
            }

            srcImage.UnlockBits(srcData);
            dstImage.UnlockBits(destData);
            return dstImage;
        }

        private static Bitmap ScaleImageIncrementally_(Bitmap src, int targetWidth, int targetHeight)
        {
            //bool hasReassignedSrc = false;

            int currentWidth = src.Width;
            int currentHeight = src.Height;

            // For ultra quality should use 7
            int fraction = 2;

            do
            {
                int prevCurrentWidth = currentWidth;
                int prevCurrentHeight = currentHeight;

                // If the current width is bigger than our target, cut it in half and sample again.
                if (currentWidth > targetWidth)
                {
                    currentWidth -= (currentWidth / fraction);

                    // If we cut the width too far it means we are on our last iteration. Just set it to the target width and finish up.
                    if (currentWidth < targetWidth)
                        currentWidth = targetWidth;
                }

                // If the current height is bigger than our target, cut it in half and sample again.
                if (currentHeight > targetHeight)
                {
                    currentHeight -= (currentHeight / fraction);

                    // If we cut the height too far it means we are on our last iteration. Just set it to the target height and finish up.
                    if (currentHeight < targetHeight)
                        currentHeight = targetHeight;
                }

                // Stop when we cannot incrementally step down anymore.
                if (prevCurrentWidth == currentWidth && prevCurrentHeight == currentHeight)
                    break;

                // Render the incremental scaled image.
                Bitmap incrementalImage = ScaleImageBicubic_(src, currentWidth, currentHeight);

                // Before re-assigning our interim (partially scaled) incrementalImage to be the new src image before we iterate around
                // again to process it down further, we want to flush() the previous src image IF (and only IF) it was one of our own temporary
                // BufferedImages created during this incremental down-sampling cycle. If it wasn't one of ours, then it was the original
                // caller-supplied BufferedImage in which case we don't want to flush() it and just leave it alone.
                //      if (hasReassignedSrc)
                //          src.flush();

                // Now treat our incremental partially scaled image as the src image
                // and cycle through our loop again to do another incremental scaling of it (if necessary).
                src = incrementalImage;

                // Keep track of us re-assigning the original caller-supplied source image with one of our interim BufferedImages
                // so we know when to explicitly flush the interim "src" on the next cycle through.
                //hasReassignedSrc = true;
            } while (currentWidth != targetWidth || currentHeight != targetHeight);
            return src;
        }

        public static byte[] EncodeAsPng(Bitmap image)
        {
            using (var byteStream = new MemoryStream())
            {
                image.Save(byteStream, ImageFormat.Png);
                return byteStream.ToArray();
            }
        }

        public static Bitmap CreateBitmap(byte[] bytes)
        {
            using (var byteStream = new MemoryStream(bytes))
            {
                return new Bitmap(byteStream);
            }
        }

        public static Bitmap Crop(Bitmap source, Rectangle rect)
        {
            Bitmap target = new Bitmap(rect.Width, rect.Height, source.PixelFormat);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(source, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            }
            return target;
        }

        public static string ComputeImageHash(Bitmap image)
        {
            using (var byteStream = new MemoryStream())
            {
                image.Save(byteStream, ImageFormat.Bmp);
                byte[] bytes = byteStream.ToArray();
                return CommonUtils.GetSha256Hash(bytes);
            }
        }

        #endregion

        #endregion
    }
}
