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
        private static readonly Regex SubtitleRegex = AniCharades.Common.Titles.TitleRegularExpressions.LooseSubtitleRegex;
        private static readonly Regex CollaborationRegex = AniCharades.Common.Titles.TitleRegularExpressions.CollaborationRegex;

        private static readonly string[] NonImportantOthers = { "CM", "CMs" };
        private static readonly string[] NonImportantTypes = { "Music" };

        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            if (!secondEntry.Related.AllRelatedPositions.Any(r => r.MalId == firstEntry.Id))
                return false;
            if (NonImportantOthers.Any(nio => secondEntry.Title.Contains(nio)))
                return true;
            if (IsCollaboration(firstEntry, secondEntry))
                return false;
            if (secondEntry.Description.Contains(firstEntry.Title) && firstEntry.Title.ContainsAnySharedWord(secondEntry.Title))
                return true;
            return BothEntiresContainEveryWord(firstEntry, secondEntry);
        }

        private bool IsCollaboration(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            if (secondEntry.Related.AllRelatedPositions.Count > 1 &&
                CollaborationRegex.IsMatch(secondEntry.Title))
            {
                var match = CollaborationRegex.Match(secondEntry.Title);
                var collabFirstTitle = match.Groups["firstPart"].Value;
                var collabSecondTitle = match.Groups["secondPart"].Value;
                if (firstEntry.Title.Equals(collabFirstTitle) || firstEntry.Title.Equals(collabSecondTitle))
                    return true;
            }
            return false;
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
            return SubtitleRegex.IsMatch(firstEntry.Title) && SubtitleRegex.IsMatch(secondEntry.Title);
        }

        private string GetMainTitlePart(string title)
        {
            var match = SubtitleRegex.Match(title);
            var mainTitle = match.Groups["mainTitle"].Value;
            return mainTitle;
        }
    }
}
