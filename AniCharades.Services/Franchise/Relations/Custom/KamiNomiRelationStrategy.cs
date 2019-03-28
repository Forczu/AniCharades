using System;
using System.Collections.Generic;
using System.Text;
using AniCharades.Adapters.Interfaces;

namespace AniCharades.Services.Franchise.Relations.Custom
{
    public class KamiNomiRelationStrategy : IRelationStrategy
    {
        private static readonly string KamiNomiKeyword = "kami nomi zo";
        private static readonly string MagicalKanonKeyword = "magical☆star";

        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            string firstTitle = firstEntry.Title.ToLower();
            string secondTitle = secondEntry.Title.ToLower();
            return IsKamiNomiOrMagicalKanon(firstTitle) && IsKamiNomiOrMagicalKanon(secondTitle);
        }

        private bool IsKamiNomiOrMagicalKanon(string title)
        {
            return IsKamiNomiTitle(title) || IsMagicalKanonTitle(title);
        }

        private bool IsKamiNomiTitle(string title)
        {
            return title.Contains(KamiNomiKeyword);
        }

        private bool IsMagicalKanonTitle(string title)
        {
            return title.Contains(MagicalKanonKeyword);
        }
    }
}
