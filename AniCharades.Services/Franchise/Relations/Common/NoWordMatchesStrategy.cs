using System;
using System.Collections.Generic;
using System.Text;
using AniCharades.Adapters.Interfaces;

namespace AniCharades.Services.Franchise.Relations.Common
{
    public class NoWordMatchesStrategy : IRelationStrategy
    {
        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            return true;
        }
    }
}
