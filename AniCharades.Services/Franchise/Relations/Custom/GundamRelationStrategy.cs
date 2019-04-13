using System.Text.RegularExpressions;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Common.Utils;

namespace AniCharades.Services.Franchise.Relations.Custom
{
    public class GundamRelationStrategy : IRelationStrategy
    {
        private static Regex GNoReconguistaSeries = new Regex("g[^a-z]|reconguista");
        private static readonly string MobileFighterGGundamSingleTitle = "mobile fighter g gundam";
        private static readonly string IronBloodedOrphansSeries = "Iron-Blooded";
        private static readonly string KimiGaNozomuEienSeries = "kimi ga nozomu eien";

        public bool AreRelated(IEntryInstance firstEntry, IEntryInstance secondEntry)
        {
            string firstTitle = firstEntry.Title;
            string secondTitle = secondEntry.Title;
            if (IsAnyMobileFighterGGundam(firstTitle, secondTitle))
                return false;
            if (IsAnyGNoReconguista(firstTitle, secondTitle))
            {
                if (IsBothGNoReconguista(firstTitle, secondTitle))
                    return true;
                return false;
            }
            if (IsAnyIronBloodedOrphans(firstTitle, secondTitle))
            {
                if (IsBothIronBloodedOrphans(firstTitle, secondTitle))
                    return true;
                return false;
            }
            if (IsAnyKimiGaNozomuEien(firstTitle, secondTitle))
                return false;
            return firstTitle.ContainsAnySharedWord(secondTitle);
        }

        private bool IsAnyMobileFighterGGundam(string firstTitle, string secondTitle)
        {
            return firstTitle.ContainsCaseInsensitive(MobileFighterGGundamSingleTitle) ||
                secondTitle.ContainsCaseInsensitive(MobileFighterGGundamSingleTitle);
        }

        private bool IsAnyGNoReconguista(string firstTitle, string secondTitle)
        {
            return IsGNoReconguista(firstTitle) || IsGNoReconguista(secondTitle);
        }

        private bool IsBothGNoReconguista(string firstTitle, string secondTitle)
        {
            return IsGNoReconguista(firstTitle) && IsGNoReconguista(secondTitle);
        }

        private bool IsGNoReconguista(string title)
        {
            return GNoReconguistaSeries.IsMatch(title.ToLower());
        }

        private bool IsAnyIronBloodedOrphans(string firstTitle, string secondTitle)
        {
            return firstTitle.Contains(IronBloodedOrphansSeries) || secondTitle.Contains(IronBloodedOrphansSeries);
        }

        private bool IsBothIronBloodedOrphans(string firstTitle, string secondTitle)
        {
            return IsIronBloodedOrphans(firstTitle) && IsIronBloodedOrphans(secondTitle);
        }

        private bool IsIronBloodedOrphans(string title)
        {
            return title.ContainsCaseInsensitive(IronBloodedOrphansSeries);
        }

        private bool IsAnyKimiGaNozomuEien(string firstTitle, string secondTitle)
        {
            return firstTitle.ContainsCaseInsensitive(KimiGaNozomuEienSeries) || 
                secondTitle.ContainsCaseInsensitive(KimiGaNozomuEienSeries);
        }
    }
}
