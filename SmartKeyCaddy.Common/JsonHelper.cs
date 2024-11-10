using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SmartKeyCaddy.Common.JsonHelper
{
    public class JsonHelper
    {
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling= NullValueHandling.Ignore
            };
        }

        //System.Test.Json: Used in functions apps
        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public static string FormatDeviceResponse(string responseJson)
        {
            var formattedJson = Regex.Unescape(responseJson);

            if (formattedJson.StartsWith("\"") && formattedJson.EndsWith("\""))
                formattedJson = formattedJson.Substring(1, formattedJson.Length - 2);

            return formattedJson;
        }
    }
}
