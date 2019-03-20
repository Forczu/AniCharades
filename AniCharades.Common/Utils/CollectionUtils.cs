using System.Collections.Generic;
using System.Linq;

namespace AniCharades.Common.Utils
{
    public static class CollectionUtils
    {
        public static ICollection<T> MergeCollectionsWithoutNullValues<T>(ICollection<T>[] collections)
        {
            var mergedCollection = new List<T>();
            foreach (var collection in collections.Where(c => c != null))
            {
                mergedCollection.AddRange(collection);
            }
            return mergedCollection;
        }
    }
}
