namespace SmartKeyCaddy.Tests
{
    public class Helper
    {
        public static string GetDeviceRegisterMessage()
        {
            using var reader = new StreamReader(@$"Messages\DeviceRegisterMessage.json");

            return reader.ReadToEnd();
        }
    }
}
