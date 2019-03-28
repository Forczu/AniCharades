using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Utils;

namespace AniCharades.Services.Franchise.Relations.Common
{
    public class AnyWordMatchesStrategy : IRelationStrategy
    {
        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            return firstEntry.Title.ContainsAnySharedWord(secondEntry.Title);
        }
    }
}
