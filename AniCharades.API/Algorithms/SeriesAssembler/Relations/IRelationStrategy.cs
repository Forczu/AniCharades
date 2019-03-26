using AniCharades.Adapters.Interfaces;

namespace AniCharades.API.Algorithms.SeriesAssembler.Relations
{
    public interface IRelationStrategy
    {
        bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry);
    }
}
