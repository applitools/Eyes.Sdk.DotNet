namespace Applitools.VisualGrid
{
    public class HashObject
    {
        public HashObject(string hashFormat, string hash)
        {
            HashFormat = hashFormat;
            Hash = hash;
        }

        public string HashFormat { get; }
        public string Hash { get; }
    }
}