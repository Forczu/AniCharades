using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static void IncrementValue<TKey>(this Dictionary<TKey, int> dict, TKey key)
        {
            if (dict.ContainsKey(key))
                dict[key]++;
            else
                dict.Add(key, 1);
        }
    }
}
