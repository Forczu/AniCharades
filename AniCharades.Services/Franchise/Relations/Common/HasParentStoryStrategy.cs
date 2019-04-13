using System.Linq;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Data.Enumerations;

namespace AniCharades.Services.Franchise.Relations.Common
{
    public class HasParentStoryStrategy : IRelationStrategy
    {
        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            var firstIsParentStoryForSecond = secondEntry.Related.AllRelatedPositions
                .Any(r => r.MalId == firstEntry.Id && 
                    (r.RelationType.In(RelationType.ParentStory, RelationType.FullStory, RelationType.SideStory)));
            return firstIsParentStoryForSecond;
        }
    }
}
