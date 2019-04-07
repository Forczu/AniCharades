using AniCharades.Adapters.Interfaces;
using AniCharades.Data.Enumerations;

namespace AniCharades.Adapters.Jikan
{
    public class RelationBetweenEntries
    {
        public IEntryInstance SourceEntry { get; set; }

        public IEntryInstance TargetEntry { get; set; }

        public RelationType SourceToTargetType { get; set; }

        public RelationType TargetToSourceType { get; set; }

        public RelationBetweenEntries(IEntryInstance source, IEntryInstance related, RelationType sourceToTargetType,
            RelationType targetToSourceType = RelationType.None)
        {
            SourceEntry = source;
            TargetEntry = related;
            SourceToTargetType = sourceToTargetType;
            TargetToSourceType = targetToSourceType;
        }
    }
}
