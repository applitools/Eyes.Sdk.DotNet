using Applitools.VisualGrid.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Applitools.VisualGrid
{
    public class FileDebugResourceWriter : IDebugResourceWriter
    {
        private int counter_ = 0;
        private readonly object lockObject_ = new object();
        private static readonly Dictionary<string, string[]> extensions = new Dictionary<string, string[]>()
        {
            { "image/svg+xml", new string[] { "svg" } },
            { "image/jpeg", new string[] { "jpg", "jpeg" } },
            { "image/tiff", new string[] { "tif", "tiff" } },

            { "audio/mpeg", new string[] { "mp3" } },
            { "video/mpeg", new string[] { "mpg", "mpeg" } },

            { "audio/ogg", new string[] { "oga" } },
            { "video/ogg", new string[] { "ogv" } },
            { "application/ogg", new string[] { "ogx" } },

            { "video/x-msvideo", new string[] { "avi" } },

            { "application/x-font-otf", new string[] { "otf" } },
            { "application/x-font-woff", new string[] { "woff" } },
            { "application/x-font-woff2", new string[] { "woff2" } },
            { "application/x-font-ttf", new string[] { "ttf" } },
            { "application/vnd.ms-fontobject", new string[] { "eot" } }

        };

        public FileDebugResourceWriter(string targetFolder)
        {
            TargetFolder = targetFolder;
        }

        public string TargetFolder { get; set; }

        public void Write(RGridResource value)
        {
            if (value.Content == null || value.Content.Length == 0 || value.ContentType == null) return;
            string filename;
            lock (lockObject_)
            {
                if (!Directory.Exists(TargetFolder))
                {
                    Directory.CreateDirectory(TargetFolder);
                }
                //filename = Path.Combine(TargetFolder, Interlocked.Increment(ref counter_) + "_" + value.Sha256);
                string ext = value.ContentType;
                string ext2 = null;
                int semicolon = ext.IndexOf(";");
                if (semicolon > -1)
                {
                    ext = ext.Substring(0, semicolon);
                }

                if (extensions.TryGetValue(ext, out string[] mimeExtensions))
                {
                    ext = mimeExtensions[0];
                    if (mimeExtensions.Length > 1)
                    {
                        ext2 = mimeExtensions[1];
                    }
                }
                else
                {
                    int slash = ext.IndexOf("/");
                    ext = ext.Substring(slash + 1);
                }

                string lastSegment = value.Url.Segments.Last();

                if ("octet-stream".Equals(ext, System.StringComparison.OrdinalIgnoreCase) ||
                    lastSegment.EndsWith("." + ext) || (ext2 != null && lastSegment.EndsWith("." + ext2)))
                {
                    filename = Path.Combine(TargetFolder, Interlocked.Increment(ref counter_) + "_" + lastSegment);
                }
                else
                {
                    filename = Path.Combine(TargetFolder, Interlocked.Increment(ref counter_) + "_" + value.Url.Host);
                    filename += "." + ext;
                }
            }
            File.WriteAllBytes(filename, value.Content);
        }

        public void Write(FrameData value)
        {
            if (value == null) return;
            lock (lockObject_)
            {
                if (!Directory.Exists(TargetFolder))
                {
                    Directory.CreateDirectory(TargetFolder);
                }
                string filename = Path.Combine(TargetFolder, Interlocked.Increment(ref counter_) + "_dom_snapshot.json");
                string data = JsonConvert.SerializeObject(value, Formatting.Indented);
                File.WriteAllText(filename, data);
            }
        }
    }
}