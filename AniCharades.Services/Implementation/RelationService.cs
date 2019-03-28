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
            var relationCriteria = RelationConfiguration.Instance.Get(relation.SourceEntry.Title, relation.Type);
            var relationStrategy = RelationFactory.Instance.Create(relationCriteria.Strategy);
            var areEqual = relationStrategy.AreRelated(relation.SourceEntry, relation.TargetEntry);
            return areEqual;
        }
    }
}
