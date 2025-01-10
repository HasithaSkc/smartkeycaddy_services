
using Newtonsoft.Json;
using SmartKeyCaddy.Common.JsonHelper;
using SmartKeyCaddy.Models;
using System.Text.RegularExpressions;

namespace SmartKeyCaddy.Domain.Services;

public static class ServiceHelper
{
    public static DeviceKeyAllocationResponse GetDeviceKeyAllocationResponse(string responseJson)
    {
        var formattedJson = Regex.Unescape(responseJson);

        if (formattedJson.StartsWith("\"") && formattedJson.EndsWith("\""))
            formattedJson = formattedJson.Substring(1, formattedJson.Length - 2);

        return JsonConvert.DeserializeObject<DeviceKeyAllocationResponse>(formattedJson, JsonHelper.GetJsonSerializerSettings());
    }

    public static bool DeviceLogResponse(string responseJson)
    {
        var formattedJson = Regex.Unescape(responseJson);

        if (formattedJson.StartsWith("\"") && formattedJson.EndsWith("\""))
            formattedJson = formattedJson.Substring(1, formattedJson.Length - 2);

        return JsonConvert.DeserializeObject<bool>(formattedJson, JsonHelper.GetJsonSerializerSettings());
    }
}