using System.Linq;

namespace AniCharades.Common.Extensions
{
    public static class StructExtensions
    {
        public static bool In<T>(this T source, params T[] targets) where T : struct
        {
            return targets.Any(o => source.Equals(o));
        }

        public static bool NotIn<T>(this T source, params T[] targets) where T : struct
        {
            return !source.NotIn(targets);
        }
    }
}
