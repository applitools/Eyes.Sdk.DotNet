namespace Applitools
{
    public class FileDebugResourceWriter : IDebugResourceWriter
    {
        public FileDebugResourceWriter(string targetFolder)
        {
            TargetFolder = targetFolder;
        }

        public string TargetFolder { get; set; }
    }
}
