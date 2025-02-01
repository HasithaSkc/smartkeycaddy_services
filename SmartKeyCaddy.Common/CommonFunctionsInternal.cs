namespace SmartKeyCaddy.Common;

public static partial class CommonFunctions
{
    private static string GenerateRandomKeyPinCode(int noDigits)
    {
        if (noDigits <= 0)
            throw new ArgumentException("Number of digits must be greater than 0.");

        int minValue = (int)Math.Pow(10, noDigits - 1);
        int maxValue = (int)Math.Pow(10, noDigits) - 1;

        var random = new Random();
        return random.Next(minValue, maxValue + 1).ToString();
    }
}