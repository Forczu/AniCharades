using AniCharades.Adapters.Jikan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Services.Interfaces
{
    public interface IRelationService
    {
        ICollection<long> GetAnimeRelations(long malId);

        bool IsRelationValid(RelationBetweenEntries relation);
    }
}
