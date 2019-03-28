using AniCharades.Adapters.Interfaces;

namespace AniCharades.Services.Franchise.Relations
{
    public interface IRelationStrategy
    {
        bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry);
    }
}
