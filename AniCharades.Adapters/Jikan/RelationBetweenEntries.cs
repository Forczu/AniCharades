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

        /// <summary>
        /// Constructor for one entry, indicates an entry without a relation
        /// </summary>
        public RelationBetweenEntries(IEntryInstance entry)
        {
            SourceEntry = entry;
            TargetEntry = null;
            TargetForSourceType = SourceForTargetType = RelationType.None;
        }

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
