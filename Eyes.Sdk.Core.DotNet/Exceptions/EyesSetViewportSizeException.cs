using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Applitools
{
    [Serializable]
    public class EyesSetViewportSizeException : EyesException
    {
        public EyesSetViewportSizeException(Size actualViewportSize, bool isRoundingError)
            : base($"Could not set required viewport size. {(isRoundingError ? "Seems like a rounding error. " : string.Empty)} Actual viewport size: {actualViewportSize}.")
        {
            ActualViewportSize = actualViewportSize;
            IsRoundingError = isRoundingError;
        }

        public Size ActualViewportSize { get; }
        public bool IsRoundingError { get; }
    }
}