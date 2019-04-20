using System.Collections.Generic;

namespace AniCharades.Common.Extensions
{
    public static class QueueExtensions
    {
        public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enumerable)
        {
            foreach (T obj in enumerable)
            {
                queue.Enqueue(obj);
            }
        }
    }
}
