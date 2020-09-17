using System;
using System.Drawing;

namespace Applitools.Images
{
    public class ImagesCheckSettings : CheckSettings, IImagesCheckTarget
    {
        private Bitmap image_;
        private Uri imageUri_;

        public ImagesCheckSettings(Bitmap image)
        {
            this.image_ = image;
        }
        public ImagesCheckSettings(Uri imageUri)
        {
            this.imageUri_ = imageUri;
        }

        private ImagesCheckSettings() { }

        Bitmap IImagesCheckTarget.Image => this.image_;

        Uri IImagesCheckTarget.ImageUri => this.imageUri_;

        public ImagesCheckSettings Region(Rectangle region)
        {
            ImagesCheckSettings clone = Clone_();
            clone.UpdateTargetRegion(region);
            return clone;
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
