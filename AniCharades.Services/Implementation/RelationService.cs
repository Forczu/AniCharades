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
            var relationCriteria = RelationConfiguration.Instance.Get(relation);
            var relationStrategies = relationCriteria.Strategies.Select(s => RelationFactory.Instance.Get(s)).ToArray();
            var isTargetParentStory = relation.TargetForSourceType == Data.Enumerations.RelationType.ParentStory;
            bool areEqual = true;
            foreach (var relationStrategy in relationStrategies)
            {
                areEqual &= !isTargetParentStory
                    ? relationStrategy.AreRelated(relation.SourceEntry, relation.TargetEntry)
                    : relationStrategy.AreRelated(relation.TargetEntry, relation.SourceEntry);
            }
            return areEqual;
        }
    }
}
