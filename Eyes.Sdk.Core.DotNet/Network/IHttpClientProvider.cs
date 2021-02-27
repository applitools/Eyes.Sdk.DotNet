using System.Net.Http;

namespace Applitools.Utils
{
    public interface IHttpClientProvider
    {
        HttpClient GetClient();
    }
}