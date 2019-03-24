using AniCharades.Data.Enumerations;

namespace AniCharades.Data.Models
{
    public class RelatedSubItem
    {
        public long MalId { get; }

        public RelationType RelationType { get; }

        public RelatedSubItem(long malId, RelationType relationType)
        {
            MalId = malId;
            RelationType = relationType;
        }
    }
}
