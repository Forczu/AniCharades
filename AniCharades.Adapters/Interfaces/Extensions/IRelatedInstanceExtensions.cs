using AniCharades.Data.Enumerations;
using AniCharades.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AniCharades.Adapters.Interfaces.Extensions
{
    public static class IRelatedInstanceExtensions
    {
        internal static void CreateAllRelatedPositionsCollection(this IRelatedInstance relatedInstance)
        {
            var allRelatedCollections = new[]
            {
                new { RelatedCollection = relatedInstance.AlternativeVersions, RelationType = RelationType.AlternativeVersion },
                new { RelatedCollection = relatedInstance.Characters, RelationType = RelationType.Characters },
                new { RelatedCollection = relatedInstance.Others, RelationType = RelationType.Other },
                new { RelatedCollection = relatedInstance.Prequels, RelationType = RelationType.Prequel },
                new { RelatedCollection = relatedInstance.Sequels, RelationType = RelationType.Sequel },
                new { RelatedCollection = relatedInstance.SideStories, RelationType = RelationType.SideStory },
                new { RelatedCollection = relatedInstance.SpinOffs, RelationType = RelationType.SpinOff },
                new { RelatedCollection = relatedInstance.Summaries, RelationType = RelationType.Summary },
            };
            relatedInstance.AllRelatedPositions = CreateAllRelatedPositions(allRelatedCollections);
        }

        private static ICollection<RelatedSubItem> CreateAllRelatedPositions(dynamic[] relatedCollections)
        {
            var allRelatedEntries = new List<RelatedSubItem>();
            foreach (var relatedCollection in relatedCollections.Where(rc => rc.RelatedCollection != null))
            {
                foreach (var relatedItem in relatedCollection.RelatedCollection)
                {
                    allRelatedEntries.Add(new RelatedSubItem(relatedItem.MalId, relatedCollection.RelationType));
                }
            }
            return allRelatedEntries;
        }
    }
}
