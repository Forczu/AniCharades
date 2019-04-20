using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> objects)
        {
            foreach (var item in objects)
            {
                source.Add(item);
            }
        }
    }
}
