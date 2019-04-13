using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Utils;

namespace AniCharades.Services.Franchise.Relations.Common
{
    public class OtherRelationStrategy : IRelationStrategy
    {
        private static readonly Regex subtitleRegex = AniCharades.Common.Titles.TitleRegularExpressions.LooseSubtitleRegex;
        private static readonly string[] NonImportantOthers = { "CM", "CMs" };
        private static readonly string[] NonImportantTypes = { "Music" };

        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            if (!secondEntry.Related.AllRelatedPositions.Any(r => r.MalId == firstEntry.Id))
                return false;
            if (NonImportantOthers.Any(nio => secondEntry.Title.Contains(nio)))
                return true;
            return BothEntiresContainEveryWord(firstEntry, secondEntry);
        }

        private bool BothEntiresContainEveryWord(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            string firstTitle = firstEntry.Title, secondTitle = secondEntry.Title;
            if (AreBothEntriesAreSubtitles(firstEntry, secondEntry))
            {
                firstTitle = GetMainTitlePart(firstTitle);
                secondTitle = GetMainTitlePart(secondTitle);
            }
            if (!firstTitle.ContainsEverySharedWord(secondTitle))
                return false;
            return true;
        }

        private bool AreBothEntriesAreSubtitles(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            return subtitleRegex.IsMatch(firstEntry.Title) && subtitleRegex.IsMatch(secondEntry.Title);
        }

        private string GetMainTitlePart(string title)
        {
            var match = subtitleRegex.Match(title);
            var mainTitle = match.Groups["mainPart"].Value;
            return mainTitle;
        }
    }
}
