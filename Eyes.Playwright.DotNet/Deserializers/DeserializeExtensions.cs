using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Applitools.Playwright
{
    public static class DeserializeExtensions
    {
        public static T ToObject<T>(this JsonElement? element)
        {
            if (element == null)
            {
                return default;
            }

            var json = element.Value.GetRawText();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T ToObject<T>(this JsonDocument document)
        {
            if (document == null)
            {
                return default;
            }

            var json = document.RootElement.GetRawText();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}