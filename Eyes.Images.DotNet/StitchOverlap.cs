namespace Applitools
{
    public class StitchOverlap
    {
        public int? Top { get; set; }
        public int? Bottom { get; set; }
    
        public StitchOverlap() {}
    
        public StitchOverlap(StitchOverlap stitchOverlap)
        {
            this.Top = stitchOverlap.Top;
            this.Bottom = stitchOverlap.Bottom;
        }
    }
}