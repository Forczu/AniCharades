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
            ChangeTitlesIfBothEntriesAreSubtitles(firstEntry, secondEntry, out string firstTitle, out string secondTitle);
            if (!firstTitle.ContainsEverySharedWord(secondTitle))
                return false;
            return true;
        }

        private void ChangeTitlesIfBothEntriesAreSubtitles(IEntryInstance firstEntry, IEntryInstance secondEntry, out string firstTitle, out string secondTitle)
        {
            firstTitle = firstEntry.Title;
            secondTitle = secondEntry.Title;
            foreach (var separator in new char[] { ':', '!', '?' })
            {
                if (BothEntriesAreSubtitles(firstEntry, secondEntry, separator))
                {
                    firstTitle = GetMainTitlePart(firstTitle, separator);
                    secondTitle = GetMainTitlePart(secondTitle, separator);
                    break;
                }
            }
        }

        private bool BothEntriesAreSubtitles(IEntryInstance firstEntry, IEntryInstance secondEntry, char separator)
        {
            return firstEntry.Title.Contains(separator) && secondEntry.Title.Contains(separator);
        }

        private string GetMainTitlePart(string title, char separator)
        {
            return title.Substring(0, title.IndexOf(separator));
        }
    }
}
