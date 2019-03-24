using AniCharades.Data.Enumerations;

namespace AniCharades.Data.Models
{
    public class RelatedItem
    {
        public long MalId { get; }

        public RelationType RelationType { get; }

        public RelatedItem(long malId, RelationType relationType)
        {
            MalId = malId;
            RelationType = relationType;
        }
    }
}
