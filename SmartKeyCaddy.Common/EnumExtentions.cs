using System;
using System.ComponentModel;

namespace SmartKeyCaddy.Common
{
    public static class EnumExtensions
    {
        public static string GetEnumString<TEnum>(Enum enumValue) where TEnum : Enum
        {
            return Enum.GetName(typeof(TEnum), enumValue) ?? string.Empty;
        }

        public static string GetEnumDescription<T>(string value)
        {
            Type type = typeof(T);
            var name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase)).Select(d => d).FirstOrDefault();

            if (name == null)
            {
                return string.Empty;
            }
            var field = type.GetField(name);
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
        }

        public static T GetEnumValue<T>(string input, T defaultValue) where T : struct, Enum
        {
            return Enum.TryParse(input, true, out T result) ? result : defaultValue;
        }
    }
}
