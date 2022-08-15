using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Applitools
{
    /// <summary>
    /// Extracts text in the specified language from the input image region.
    /// </summary>
    public delegate string[] GetTextHandler(string imageId, IList<Rectangle> regions, string lanuage);

    /// <summary>
    /// Creates an image of the specified region of the application window and returns its id.
    /// </summary>
    /// <param name="region">Image region or an empty rectangle to create an image of 
    /// the entire window.</param>
    public delegate string CreateImageHandler(Rectangle region);

    //    /// <summary>
    //    /// Applitools Eyes base class.
    //    /// </summary>
    [ComVisible(true)]
    public abstract class EyesBase : EyesBaseConfig//, IEyesBase, IBatchCloser
    {
        public override bool IsDisabled
        {
            get => Runner.IsDisabled;
            set => Runner.IsDisabled = value;
        }

        protected EyesBase(EyesRunner runner = null, Configuration configuration = null, ILogHandler logHandler = null)
            : base(configuration, logHandler, runner)
        {
        }
        
        public TestResults AbortIfNotClosed()
        {
            return Abort();
        }

        public abstract TestResults Abort();
    }
}
