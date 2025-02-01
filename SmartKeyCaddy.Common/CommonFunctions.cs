namespace SmartKeyCaddy.Common;

public static partial class CommonFunctions
{
    public static DateTime ConvertToLocalDateTime(DateTime dateTime, string timeZone)
    {
        try
        {
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, localTimeZone);
        }
        catch
        {
            return DateTime.UtcNow;
        }
    }

    public static string GenerateRandomKeyCode(List<string> keyPinCodeList, int noDigits)
    {
        var keyPinCode = GenerateRandomKeyPinCode(noDigits);
        while (keyPinCodeList.SingleOrDefault(key => string.Equals(key, keyPinCode, StringComparison.OrdinalIgnoreCase)) != null)
        {
            keyPinCode = GenerateRandomKeyPinCode(noDigits);
        }

        return keyPinCode;
    }
}