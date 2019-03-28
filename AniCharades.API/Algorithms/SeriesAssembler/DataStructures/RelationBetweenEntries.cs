using AniCharades.Adapters.Interfaces;
using AniCharades.Data.Enumerations;

namespace AniCharades.API.Algorithms.SeriesAssembler.DataStructures
{
    public class RelationBetweenEntries
    {
        public IEntryInstance SourceEntry { get; set; }

        public IEntryInstance TargetEntry { get; set; }

        public RelationType Type { get; set; }

        public RelationBetweenEntries(IEntryInstance source, IEntryInstance related, RelationType relationType)
        {
            SourceEntry = source;
            TargetEntry = related;
            Type = relationType;
        }
    }
}
