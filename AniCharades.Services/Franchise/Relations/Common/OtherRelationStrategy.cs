using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Utils;

namespace AniCharades.Services.Franchise.Relations.Common
{
    public class OtherRelationStrategy : IRelationStrategy
    {
        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            if (!secondEntry.Related.AllRelatedPositions.Any(r => r.MalId == firstEntry.Id))
                return false;
            string firstTitle = firstEntry.Title, secondTitle = secondEntry.Title;
            if (BothEntriesAreSubtitles(firstEntry, secondEntry))
            {
                firstTitle = GetMainTitlePart(firstTitle);
                secondTitle = GetMainTitlePart(secondTitle);
            }
            if (!firstTitle.ContainsEverySharedWord(secondTitle))
                return false;
            return true;
        }

        private bool BothEntriesAreSubtitles(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            return firstEntry.Title.Contains(':') && secondEntry.Title.Contains(':');
        }

        private string GetMainTitlePart(string title)
        {
            return title.Substring(0, title.IndexOf(':'));
        }
    }
}
