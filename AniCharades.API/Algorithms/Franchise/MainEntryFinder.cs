using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.Franchise
{
    public static class MainEntryFinder
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
            var sortedSeries = series.Where(s => s.TimePeriod.From != null).OrderBy(a => a.Id).ToList();
            IEntryInstance mainEntry = series.First(), otherEntry = null;
            if (IsSecondaryTitle(mainEntry))
            {
                otherEntry = series.Where(a => !IsSecondaryTitle(a) && a != mainEntry).FirstOrDefault();
                if (otherEntry != null)
                {
                    mainEntry = otherEntry;
                }
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
    }
}
