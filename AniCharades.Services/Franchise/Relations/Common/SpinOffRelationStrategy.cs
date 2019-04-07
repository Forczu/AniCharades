using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Utils;

namespace AniCharades.Services.Franchise.Relations.Common
{
    public class SpinOffRelationStrategy : IRelationStrategy
    {
        private static readonly string[] NonImportantTypes = { "OVA", "Special", "Music" };

        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            if (firstEntry.Title.ContainsAnySharedWord(secondEntry.Title))
                return true;
            if (NonImportantTypes.Contains(secondEntry.Type))
                return true;
            return false;
        }
    }
}
