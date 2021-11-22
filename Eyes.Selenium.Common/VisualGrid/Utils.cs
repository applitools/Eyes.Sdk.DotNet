namespace Applitools.VisualGrid
{
    public static class Utils
    {
        internal static Ufg.IDebugResourceWriter ConvertDebugResourceWriter(IDebugResourceWriter drw)
        {
            Ufg.IDebugResourceWriter ufgDrw = null;
            if (drw is FileDebugResourceWriter fileDRW)
            {
                ufgDrw = new Ufg.FileDebugResourceWriter(fileDRW.TargetFolder);
            }
            ufgDrw = ufgDrw ?? Ufg.NullDebugResourceWriter.Instance;
            return ufgDrw;
        }
    }
}
