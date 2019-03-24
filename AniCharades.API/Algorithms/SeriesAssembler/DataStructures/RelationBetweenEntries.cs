using AniCharades.Adapters.Interfaces;
using AniCharades.Data.Enumerations;

namespace AniCharades.API.Algorithms.SeriesAssembler.DataStructures
{
    public class RelationBetweenEntries<T> where T : IEntryInstance
    {
        public T SourceEntry { get; set; }

        public T TargetEntry { get; set; }

        public RelationType Type { get; set; }

        public RelationBetweenEntries(T source, T related, RelationType relationType)
        {
            SourceEntry = source;
            TargetEntry = related;
            Type = relationType;
        }
    }
}
