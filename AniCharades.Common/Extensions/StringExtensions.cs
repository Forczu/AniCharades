using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsCaseInsensitive(this string text, string value,
              StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }
    }
}
