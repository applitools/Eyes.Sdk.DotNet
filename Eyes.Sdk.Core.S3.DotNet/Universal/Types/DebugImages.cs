namespace Applitools
{
    public class DebugImages
    {
        public string Path { get; }
        public string Prefix { get; }

        public DebugImages(string path, string prefix)
        {
            Path = path;
            Prefix = prefix;
        }
    }
}