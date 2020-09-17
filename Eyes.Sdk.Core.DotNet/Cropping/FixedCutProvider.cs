using Applitools.Utils.Cropping;
using System;
using System.Drawing;
using Applitools.Utils.Images;

namespace Applitools.Cropping
{
    public class FixedCutProvider : ICutProvider
    {
        protected readonly int header_;
        protected readonly int footer_;
        protected readonly int left_;
        protected readonly int right_;

        public FixedCutProvider(int header, int footer, int left, int right)
        {
            header_ = header;
            footer_ = footer;
            left_ = left;
            right_ = right;
        }

        public Bitmap Cut(Bitmap image)
        {
            Rectangle rect = ToRectangle(image.Size);
            return BasicImageUtils.Crop(image, rect);
        }

        public virtual ICutProvider Scale(double scaleRatio)
        {
            int scaledHeader = (int)Math.Ceiling(header_ * scaleRatio);
            int scaledFooter = (int)Math.Ceiling(footer_ * scaleRatio);
            int scaledLeft = (int)Math.Ceiling(left_ * scaleRatio);
            int scaledRight = (int)Math.Ceiling(right_ * scaleRatio);
            return new FixedCutProvider(scaledHeader, scaledFooter, scaledLeft, scaledRight);
        }

        public Rectangle ToRectangle(Size size)
        {
            Rectangle rect = new Rectangle(left_, header_,
                  size.Width - left_ - right_,
                  size.Height - header_ - footer_);

            return rect;
        }

        protected string HLFR => $"(header: {header_} ; footer: {footer_} ; left: {left_} ; right: {right_}px)";

        public override string ToString()
        {
            return $"{nameof(FixedCutProvider)} {HLFR}";
        }
    }
}
