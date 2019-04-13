using System.Linq;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;

namespace AniCharades.Services.Franchise.Relations.Custom
{
    public class DevilmanRelationStrategy : IRelationStrategy
    {
        private static readonly string[] DevilmanKeywords = { "Devilman", "Violence Jack", "Maou Dante" };

        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            string firstTitle = firstEntry.Title;
            string secondTitle = secondEntry.Title;
            if (AnyTitleContainsAnyKeyword(firstTitle, secondTitle))
                return true;
            return false;
        }

        private bool AnyTitleContainsAnyKeyword(string firstTitle, string secondTitle)
        {
            return TitleContainsAnyKeyword(firstTitle) && TitleContainsAnyKeyword(secondTitle);
        }

        private bool TitleContainsAnyKeyword(string title)
        {
            return DevilmanKeywords.Any(k => title.ContainsCaseInsensitive(k));
        }
    }
}
