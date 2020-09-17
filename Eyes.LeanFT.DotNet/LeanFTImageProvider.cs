using Applitools.Capture;
using HP.LFT.SDK;
using HP.LFT.SDK.Web;
using System;
using System.Drawing;

namespace Applitools.LeanFT
{
    class LeanFTImageProvider : IImageProvider
    {
        private ITestObject topLevelObject_;

        public LeanFTImageProvider(ITestObject testObject)
        {
            topLevelObject_ = testObject;
        }

        public Bitmap GetImage()
        {
            Bitmap screenshot;
            if (topLevelObject_ is IBrowser browserTopObj)
            {
                screenshot = (Bitmap)browserTopObj.Page.GetSnapshot();
            }
            else
            {
                if (topLevelObject_ is ITopLevelObject topObj)
                {
                    screenshot = (Bitmap)topObj.GetSnapshot();
                }
                else
                {
                    throw new ArgumentException("Top level object is of unsupported type");
                }
            }

            return screenshot;
        }
    }
}
