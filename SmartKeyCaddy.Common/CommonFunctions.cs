namespace SmartKeyCaddy.Common;

public static class CommonFunctions
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
}