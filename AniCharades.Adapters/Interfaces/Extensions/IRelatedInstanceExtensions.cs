using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.Adapters.Interfaces.Extensions
{
    public static class IRelatedInstanceExtensions
    {
        internal static void CreateAllRelatedPositionsCollection(this IRelatedInstance relatedInstance)
        {
            var allRelatedCollections = new ICollection<MALSubItem>[]
            {
                relatedInstance.AlternativeVersions,
                relatedInstance.Characters,
                relatedInstance.Prequels,
                relatedInstance.Others,
                relatedInstance.Sequels,
                relatedInstance.SideStories,
                relatedInstance.SpinOffs,
                relatedInstance.Summaries
            };
            relatedInstance.AllRelatedPositions = Common.Utils.CollectionUtils.MergeCollectionsWithoutNullValues(allRelatedCollections);
        }
    }
}
