using AniCharades.Adapters.Interfaces;

namespace AniCharades.API.Algorithms.SeriesAssembler.Relations
{
    public interface IRelationStrategy
    {
        bool AreEqual(IEntryInstance firstEntry, IEntryInstance secondEntry);
    }
}
