using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Applitools.Utils;

namespace Applitools
{
    public class VisualLocatorSettings
    {
        private string _image;
        private Bitmap _bitmapImage;
        private bool _firstOnly;
        private List<string> _names = new List<string>();
        
        public VisualLocatorSettings Name(string name)
        {
            VisualLocatorSettings clone = Clone();
            clone._names.Add(name);
            return clone;
        }

        public VisualLocatorSettings Names(List<string> names)
        {
            VisualLocatorSettings clone = Clone();
            clone._names.AddRange(names);
            return clone;
        }

        public ICollection<string> GetNames()
        {
            return _names;
        }

        public bool GetIsFirstOnly()
        {
            return _firstOnly;
        }

        public VisualLocatorSettings Image(Bitmap image)
        {
            VisualLocatorSettings clone = Clone();
            clone._bitmapImage = image;
            return clone;
        }

        public VisualLocatorSettings Image(string image)
        {
            VisualLocatorSettings clone = Clone();
            clone._image = image;
            return clone;
        }

        public string GetImage()
        {
            return _bitmapImage != null ? _bitmapImage.ToBase64() : _image;
        }
        
        private VisualLocatorSettings Clone()
        {
            var clone = new VisualLocatorSettings
            {
                _names = _names.ToList(),
                _firstOnly = _firstOnly,
                _bitmapImage = _bitmapImage,
                _image = _image
            };

            return clone;
        }
    }
}