using AniCharades.Adapters.Interfaces;
using AniCharades.API.Algorithms.SeriesAssembler.DataStructures;
using AniCharades.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Logic.Interfaces
{
    public interface IRelationService
    {
        ICollection<long> GetAnimeRelations(long malId);

        bool IsRelationValid<T>(RelationBetweenEntries<T> relation) where T : IEntryInstance;
    }
}
