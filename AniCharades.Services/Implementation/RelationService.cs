using AniCharades.Adapters.Jikan;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Franchise.Relations;
using AniCharades.Services.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Services.Implementation
{
    public class RelationService : IRelationService
    {
        public bool IsRelationValid(RelationBetweenEntries relation)
        {
            var isTargetParentStory = relation.TargetForSourceType == Data.Enumerations.RelationType.ParentStory;
            var relationCriteria = !isTargetParentStory
                ? RelationConfiguration.Instance.Get(relation.TargetEntry.Type, relation.SourceEntry.Title, relation.TargetForSourceType)
                : RelationConfiguration.Instance.Get(relation.TargetEntry.Type, relation.SourceEntry.Title, relation.SourceForTargetType);
            var relationStrategy = RelationFactory.Instance.Create(relationCriteria.Strategy);
            var areEqual = !isTargetParentStory
                ? relationStrategy.AreRelated(relation.SourceEntry, relation.TargetEntry)
                : relationStrategy.AreRelated(relation.TargetEntry, relation.SourceEntry);
            return areEqual;
        }
    }
}
