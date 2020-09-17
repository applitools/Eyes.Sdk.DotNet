using Applitools.Utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

#if NET45 || NET452 || NET462 || NET472 || NET48 || NETSTANDARD2_0
using BrotliSharpLib;
#endif

namespace Applitools.VisualGrid
{
    public class ResourceFuture
    {
        public ResourceFuture(Task<WebResponse> future, Uri url, Logger logger)
        {
            Future = future;
            Url = url;
            logger_ = logger;
        }

        public ResourceFuture(RGridResource rgResource, Logger logger)
        {
            Url = rgResource.Url;
            rgResource_ = rgResource;
            logger_ = logger;
        }

        public Task<WebResponse> Future { get; }

        public Uri Url { get; internal set; }
        private RGridResource rgResource_;
        private readonly Logger logger_;
        private readonly int MaxResourceSize = 1024 * 1024 * 15;

        public RGridResource GetResource() { return rgResource_; }

        public async Task<RGridResource> Get(TimeSpan timeout)
        {
            if (rgResource_ == null)
            {
                logger_.Debug("{0} Future ref#: {1}", Url, Future.GetHashCode());
                if (await Task.WhenAny(Future, Task.Delay(timeout)) == Future)
                {
                    if (Future.Exception == null)
                    {
                        using (HttpWebResponse response = (HttpWebResponse)Future.Result)
                        {
                            try
                            {
                                using (Stream stream = response.GetResponseStream())
                                {
                                    byte[] bytes;
                                    if ("br".Equals(response.Headers[HttpResponseHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                                    {
                                        logger_.Verbose("decompressing brotli encoded resource.");
                                        using (BrotliStream bs = new BrotliStream(stream, CompressionMode.Decompress))
                                        {
                                            bytes = CommonUtils.ReadToEnd(bs, MaxResourceSize);
                                        }
                                    }
                                    else
                                    {
                                        bytes = CommonUtils.ReadToEnd(stream, MaxResourceSize);
                                    }
                                    rgResource_ = new RGridResource(Url, response.ContentType, bytes, logger_, "ResourceFuture");
                                    logger_.Debug("{0} - size: {1} bytes", rgResource_, bytes.Length);
                                }
                            }
                            catch (Exception e)
                            {
                                logger_.Log("ERROR handling Future (URL: {0}) (Ref#: {1}): {2}", Url, Future.GetHashCode(), e);
                            }
                            finally
                            {
                                response.Close();
                            }
                        }
                    }
                    else if (Future.Exception.InnerException is WebException webException)
                    {
                        logger_.Log("Error downloading URL {0}: {1}", Url, webException);
                        rgResource_ = new RGridResource(Url, "unknown/error", new byte[0], logger_, webException.Message);
                    }
                    else
                    {
                        logger_.Log("Error while downloading URL {0}: {1}", Url, Future.Exception);
                        rgResource_ = new RGridResource(Url, "unknown/error", new byte[0], logger_, Future.Exception.Message);
                    }
                }
            }

            return rgResource_;
        }
    }
}