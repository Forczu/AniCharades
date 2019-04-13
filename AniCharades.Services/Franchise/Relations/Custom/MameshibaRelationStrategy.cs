using System;
using System.Collections.Generic;
using System.Text;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;

namespace AniCharades.Services.Franchise.Relations.Custom
{
    public class MameshibaRelationStrategy : IRelationStrategy
    {
        private static readonly string MameshibaKeyword = "Mameshiba";

        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            var bothAreMameshiba = BothTitlesAreMameshiba(firstEntry, secondEntry);
            return bothAreMameshiba;
        }

        private bool BothTitlesAreMameshiba(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            return IsTitleMameshiba(firstEntry) && IsTitleMameshiba(secondEntry);
        }

        private bool IsTitleMameshiba(IEntryInstance entry)
        {
            return entry.Title.ContainsCaseInsensitive(MameshibaKeyword);
        }
    }
}
