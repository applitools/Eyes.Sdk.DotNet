using System;

namespace Applitools
{
    public class RenderingInfo
    {
        public Uri ServiceUrl { get; set; }
        public string AccessToken { get; set; }

        /// <summary>
        /// The URL to which to POST the DOM snapshot as JSON.
        /// </summary>
        /// <remarks>
        /// The result of the POST will include a 'location' field in the response header.
        /// This location should be placed in the "DomUrl" field of the AppOutput struct.
        /// </remarks>
        public Uri ResultsUrl { get; set; }

        public Uri StitchingServiceUrl { get; set; }

        public int MaxImageHeight { get; set; }
        public int MaxImageArea { get; set; }

    }
}