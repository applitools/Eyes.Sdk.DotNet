using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Applitools.Utils;

namespace Applitools
{
    public class VisualLocatorSettings
    {
        private string image_;
        private Bitmap bitmapImage_;
        private bool firstOnly_;
        private List<string> names_ = new List<string>();
        
        public VisualLocatorSettings Name(string name)
        {
            VisualLocatorSettings clone = Clone();
            clone.names_.Add(name);
            return clone;
        }

        public VisualLocatorSettings Names(List<string> names)
        {
            VisualLocatorSettings clone = Clone();
            clone.names_.AddRange(names);
            return clone;
        }

        public VisualLocatorSettings Names(params string[] names)
        {
            VisualLocatorSettings clone = Clone();
            clone.names_.AddRange(names);
            return clone;
        }

        public ICollection<string> GetNames()
        {
            return names_;
        }

        public bool GetIsFirstOnly()
        {
            return firstOnly_;
        }

        public VisualLocatorSettings Image(Bitmap image)
        {
            VisualLocatorSettings clone = Clone();
            clone.bitmapImage_ = image;
            return clone;
        }

        public VisualLocatorSettings Image(string image)
        {
            VisualLocatorSettings clone = Clone();
            clone.image_ = image;
            return clone;
        }

        public string GetImage()
        {
            return bitmapImage_ != null ? bitmapImage_.ToBase64() : image_;
        }
        
        private VisualLocatorSettings Clone()
        {
            var clone = new VisualLocatorSettings
            {
                names_ = names_.ToList(),
                firstOnly_ = firstOnly_,
                bitmapImage_ = bitmapImage_,
                image_ = image_
            };

            return clone;
        }
    }
}