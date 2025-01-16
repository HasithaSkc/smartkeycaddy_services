namespace SmartKeyCaddy.Models.Exceptions
{
    public class DeviceOfflineException : Exception
    {
        public DeviceOfflineException() : base("Device offline") { }

        public DeviceOfflineException(string message) : base(message) { }

        public DeviceOfflineException(string message, Exception inner) : base(message, inner) { }
    }

}