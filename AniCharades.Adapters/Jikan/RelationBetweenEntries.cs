using AniCharades.Adapters.Interfaces;
using AniCharades.Data.Enumerations;

namespace AniCharades.Adapters.Jikan
{
    public class RelationBetweenEntries
    {
        public IEntryInstance SourceEntry { get; set; }

        public IEntryInstance TargetEntry { get; set; }

        public RelationType TargetForSourceType { get; set; }

        public RelationType SourceForTargetType { get; set; }

        public RelationBetweenEntries(IEntryInstance source, IEntryInstance related, RelationType targetForSourceType,
            RelationType sourceForTargetType = RelationType.None)
        {
            SourceEntry = source;
            TargetEntry = related;
            TargetForSourceType = targetForSourceType;
            SourceForTargetType = sourceForTargetType;
        }
    }
}
