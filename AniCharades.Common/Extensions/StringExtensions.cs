using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool EqualsCaseInsensitive(this string text, string value)
        {
            return text.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsWithBrackets(this string text, string value)
        {
            return $"({text})".Equals(value, StringComparison.OrdinalIgnoreCase);
        }


        public static bool In(this string text, params string[] values)
        {
            return values.Any(x => x.EqualsCaseInsensitive(text));
        }

        public static bool NotIn(this string text, params string[] values)
        {
            return !text.In(values);
        }
    }
}
