using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Common.Extensions;
using AniCharades.Common.Utils;
using AniCharades.Data.Enumerations;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AniCharades.Services.Franchise
{
    public class FranchiseAssembler
    {
        private List<RelationBetweenEntries> series = new List<RelationBetweenEntries>();
        private Stack<IEntryInstance> entriesToCheckTheRelations = new Stack<IEntryInstance>();

        private readonly IRelationService relationService;

        public FranchiseAssembler(IRelationService relationService)
        {
            this.relationService = relationService;
        }

        public ICollection<RelationBetweenEntries> Assembly(long entryId, IEntryProvider entryProvider)
        {
            ResetCollections();
            if (IsIgnored(entryId, entryProvider))
                return series;
            var entry = entryProvider.Get(entryId);
            if (HasMultipleParentStories(entry))
                return series;
            AddEntryToCheckTheRelations(entry);
            while (!IsStackEmpty())
            {
                ResolveTopEntry(entryProvider);
            }
            if (series.Count == 0)
                series.Add(new RelationBetweenEntries(entry));
            return series;
        }

        private void ResetCollections()
        {
            series.Clear();
            entriesToCheckTheRelations.Clear();
        }

        private void AddEntryToCheckTheRelations(IEntryInstance entry)
        {
            entriesToCheckTheRelations.Push(entry);
        }

        private bool IsStackEmpty()
        {
            return entriesToCheckTheRelations.Count == 0;
        }

        private void ResolveTopEntry(IEntryProvider entryProvider)
        {
            var currentEntry = entriesToCheckTheRelations.Pop();
            if (currentEntry == null)
                return;
            var relations = GetRelations(currentEntry, entryProvider);
            if (IsCollectionEmpty(relations))
                return;
            relations = relations
                .Where(r => relationService.IsRelationValid(r))
                .ToList();
            series.AddRange(relations);
            StackNextRelatedEntries(relations);
        }

        private ICollection<RelationBetweenEntries> GetRelations(IEntryInstance entry, IEntryProvider entryProvider)
        {
            var allRelatedToEntry = entry.Related.AllRelatedPositions;
            if (IsCollectionEmpty(allRelatedToEntry))
                return null;
            var relations = new List<RelationBetweenEntries>();
            var filteredEntries = allRelatedToEntry.Where(r => CanEntryBeAddedToSeries(r.MalId) && !IsIgnored(r.MalId, entryProvider));
            foreach (var subItem in filteredEntries)
            {
                var relatedEntry = entryProvider.Get(subItem.MalId);
                if (relatedEntry != null && !HasMultipleParentStories(relatedEntry))
                {
                    var relation = CreateRelation(entry, relatedEntry, subItem.RelationType);
                    relations.Add(relation);
                }
            }
            return relations;
        }

        private bool IsCollectionEmpty<K>(ICollection<K> collection)
        {
            return collection == null || collection.Count == 0;
        }

        private void StackNextRelatedEntries(ICollection<RelationBetweenEntries> relations)
        {
            relations.ForEach(r => AddEntryToCheckTheRelations(r.TargetEntry));
        }

        private bool CanEntryBeAddedToSeries(long entryId)
        {
            if (entryId == 0)
                return false;
            return series.All(s => s.SourceEntry.Id != entryId && s.TargetEntry.Id != entryId);
        }

        private RelationBetweenEntries CreateRelation(IEntryInstance entry, IEntryInstance relatedEntry, RelationType relationType)
        {
            if (relationType != RelationType.ParentStory)
            {
                return new RelationBetweenEntries(entry, relatedEntry, relationType);
            }
            var entryAsRelatedSubItem = relatedEntry.Related.AllRelatedPositions.FirstOrDefault(e => e.MalId == entry.Id);
            if (entryAsRelatedSubItem == null)
            {
                return new RelationBetweenEntries(entry, relatedEntry, relationType);
            }
            var targetToSourceRelation = entryAsRelatedSubItem.RelationType;
            var relation = new RelationBetweenEntries(entry, relatedEntry, relationType, targetToSourceRelation);
            return relation;
        }

        private bool IsIgnored(long entryId, IEntryProvider entryProvider)
        {
            return entryProvider.IsIgnored(entryId);
        }

        private bool HasMultipleParentStories(IEntryInstance entry)
        {
            if (entry.Related?.ParentStories?.Count > 1)
            {
                var firstTitle = entry.Related.ParentStories.First().Name;
                var secondTitle = entry.Related.ParentStories.Last().Name;
                var areRelated = firstTitle.ContainsAnySharedWord(secondTitle);
                return !areRelated;
            }
            return false;
        }
    }
}
