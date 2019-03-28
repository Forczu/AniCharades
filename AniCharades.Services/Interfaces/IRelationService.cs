using AniCharades.Adapters.Jikan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Services.Interfaces
{
    public interface IRelationService
    {
        bool IsRelationValid(RelationBetweenEntries relation);
    }
}
