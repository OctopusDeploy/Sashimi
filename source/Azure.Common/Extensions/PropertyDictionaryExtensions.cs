using System;
using System.Collections.Generic;

namespace Sashimi.Azure.Common.Extensions
{
    public static class PropertyDictionaryExtensions
    {
        public static bool ContainsPropertyWithValue(this IDictionary<string, string> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                return false;
            }

            var value = dictionary[key];
            return value != null && !string.IsNullOrEmpty(value);
        }

        public static bool ContainsPropertyWithGuid(this IDictionary<string, string> dictionary, string key)
        {
            if (!ContainsPropertyWithValue(dictionary, key))
            {
                return false;
            }

            var guid = dictionary[key];
            return Guid.TryParse(guid, out _);
        }
    }
}