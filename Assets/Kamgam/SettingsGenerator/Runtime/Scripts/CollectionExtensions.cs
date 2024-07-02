using System.Collections.Generic;
using System.Collections;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Some really basic collection extension to shorten all sorts of checks.
    /// </summary>
    public static class CollectionExtensions
    {
        public static bool IsNull(this string text) // Okay, this is not a collection.
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool IsNull(this ICollection list)
        {
            return list == null;
        }

        public static bool IsNullOrEmpty(this ICollection list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            if (source != null)
                foreach (var _ in source) // IEnumerable has no count but this does the trick.
                    return false;

            return true;
        }

        public static bool IsIndexOutOfBounds(this ICollection list, int index)
        {
            return index < 0 || index >= list.Count;
        }
    }
}
