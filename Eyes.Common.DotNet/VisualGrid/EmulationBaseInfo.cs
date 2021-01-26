using System;

namespace Applitools.VisualGrid
{
    public abstract class EmulationBaseInfo : IEquatable<EmulationBaseInfo>
    {
        public EmulationBaseInfo(ScreenOrientation screenOrientation)
        {
            ScreenOrientation = screenOrientation;
        }

        public ScreenOrientation ScreenOrientation { get; set; }

        public bool Equals(EmulationBaseInfo other)
        {
            if (other == null) return false;
            return GetType() == other.GetType() && 
                   ScreenOrientation == other.ScreenOrientation;
        }
    }
}