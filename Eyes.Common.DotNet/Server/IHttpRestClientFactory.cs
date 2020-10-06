using Applitools.Utils;
using Newtonsoft.Json;
using System;

namespace Applitools
{
    public interface IHttpRestClientFactory
    {
        HttpRestClient Create(Uri serverUrl, string agentId, JsonSerializer jsonSerializer);
    }
}