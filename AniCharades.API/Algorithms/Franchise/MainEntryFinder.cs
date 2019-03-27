using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.Franchise
{
    internal static class MainEntryFinder
    {
        private static readonly string[] SecondaryTitleKeywords = { "TV", "OVA", "OAV", "Prologue", "Season" };
        private static readonly string[] SecondaryTypeKeywords = { "Special", "Music" };

        public static IEntryInstance GetMainEntry(ICollection<IEntryInstance> series)
        {
            if (CollectionUtils.IsCollectionNullOrEmpty(series))
                return null;
            if (CollectionUtils.HasOnlySingleElement(series))
                return series.First();
            return FindMainEntry(series);
        }

        private static IEntryInstance FindMainEntry(ICollection<IEntryInstance> series)
        {
            var sortedEntries = series.Where(s => s.TimePeriod.From != null).OrderBy(a => a.Id).ToList();
            IEntryInstance mainEntry = sortedEntries.First(), otherEntry = null;
            var rejectedEntries = new List<IEntryInstance>();
            for (int i = 0; i < series.Count; ++i)
            {
                if (IsSecondaryTitle(mainEntry))
                {
                    otherEntry = series.Where(a => !IsSecondaryTitle(a) && !rejectedEntries.Contains(a)).FirstOrDefault();
                    mainEntry = ReplaceMainEntryIfOtherNotNull(mainEntry, otherEntry, rejectedEntries);
                }
                if (IsTooShortForMainSeries(mainEntry))
                {
                    otherEntry = series.Where(a => !IsTooShortForMainSeries(a) && !rejectedEntries.Contains(a)).FirstOrDefault();
                    mainEntry = ReplaceMainEntryIfOtherNotNull(mainEntry, otherEntry, rejectedEntries);
                    continue;
                }
                break;
            }
            return mainEntry;
        }

        private static bool IsSecondaryTitle(IEntryInstance entry)
        {
            return SecondaryTitleKeywords.Any(k => entry.Title.Contains(k))
                || SecondaryTypeKeywords.Any(k => entry.Type.Contains(k))
                || IsMovieExplicitly(entry);
        }

        private static bool IsMovieExplicitly(IEntryInstance entry)
        {
            return entry.Title.Contains("Movie") && entry.Type.Contains("Movie");
        }

        private static bool IsTooShortForMainSeries(IEntryInstance entry)
        {
            return Regex.IsMatch(entry.Duration, "^[0-4] min");
        }

        private static IEntryInstance ReplaceMainEntryIfOtherNotNull(IEntryInstance mainEntry, IEntryInstance otherEntry,
            List<IEntryInstance> rejectedEntries)
        {
            if (otherEntry == null)
                return mainEntry;
            rejectedEntries.Add(mainEntry);
            return otherEntry;
        }
    }
}
