using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Utils;

namespace AniCharades.API.Algorithms.SeriesAssembler.Relations.Custom
{
    public class NyarukoRelationStrategy : IRelationStrategy
    {
        private static readonly string[] Keywords = { "haiyor", "nyaru" };

        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            bool firstCondition = firstEntry.Title.ContainsEveryKeyword(Keywords);
            bool secondCondition = secondEntry.Title.ContainsEveryKeyword(Keywords);
            return firstCondition && secondCondition;
        }
    }
}
