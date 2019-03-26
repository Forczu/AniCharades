using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Utils;

namespace AniCharades.API.Algorithms.SeriesAssembler.Relations
{
    public class AnyWordMatchesStrategy : IRelationStrategy
    {
        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            return firstEntry.Title.ContainsAnySharedWord(secondEntry.Title);
        }
    }
}
