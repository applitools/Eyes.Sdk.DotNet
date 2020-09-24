namespace Applitools.VisualGrid
{
    public abstract class EmulationBaseInfo
    {

        public EmulationBaseInfo(ScreenOrientation screenOrientation)
        {
            ScreenOrientation = screenOrientation;
        }

        public ScreenOrientation ScreenOrientation { get; set; }
    }
}