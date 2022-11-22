using System;
using System.Drawing;
using Applitools.Utils;

namespace Applitools.Images
{
    public class ImagesCheckSettings : CheckSettings, IImagesCheckTarget
    {
        private Bitmap image_;
        private Uri imageUri_;

        public ImagesCheckSettings(Bitmap image)
        {
            image_ = image;
        }

        public ImagesCheckSettings(Uri imageUri)
        {
            imageUri_ = imageUri;
        }

        private ImagesCheckSettings() { }

        Bitmap IImagesCheckTarget.Image => image_;

        Uri IImagesCheckTarget.ImageUri => imageUri_;

        public ImagesCheckSettings Region(Rectangle region)
        {
            ImagesCheckSettings clone = Clone_();
            clone.UpdateTargetRegion(region);
            return clone;
        }

        public string ToImage()
        {
            return image_ != null ? image_.ToBase64() : imageUri_.ToString();
        }

        private ImagesCheckSettings Clone_()
        {
            return (ImagesCheckSettings)Clone();
        }

        protected override CheckSettings Clone()
        {
            ImagesCheckSettings clone = new ImagesCheckSettings();
            base.PopulateClone_(clone);
            clone.image_ = this.image_;
            clone.imageUri_ = this.imageUri_;
            return clone;
        }
    }
}
