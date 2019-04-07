using System;
using System.Collections.Generic;
using System.Text;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Utils;

namespace AniCharades.Services.Franchise.Relations.Common
{
    public class EveryWordMatchesStrategy : IRelationStrategy
    {
        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            return firstEntry.Title.ContainsEverySharedWord(secondEntry.Title);
        }
    }
}
