using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Common.Extensions;
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
            var entry = entryProvider.Get(entryId);
            AddEntryToCheckTheRelations(entry);
            while (!IsStackEmpty())
            {
                ResolveTopEntry(entryProvider);
            }
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
            var validRelations = relations.Where(r => relationService.IsRelationValid(r)).ToList();
            series.AddRange(validRelations);
            StackNextRelatedEntries(validRelations);
        }

        private ICollection<RelationBetweenEntries> GetRelations(IEntryInstance entry, IEntryProvider entryProvider)
        {
            var allRelatedToEntry = entry.Related.AllRelatedPositions;
            if (IsCollectionEmpty(allRelatedToEntry))
                return null;
            var relations = new List<RelationBetweenEntries>();
            var filteredEntries = allRelatedToEntry.Where(r => CanEntryBeAddedToSeries(r.MalId));
            foreach (var subItem in filteredEntries)
            {
                var relatedEntry = entryProvider.Get(subItem.MalId);
                if (relatedEntry != null)
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
            return series.All(s => s.SourceEntry.Id != entryId && s.TargetEntry.Id != entryId);
        }

        private RelationBetweenEntries CreateRelation(IEntryInstance entry, IEntryInstance relatedEntry, RelationType relationType)
        {
            if (relationType != RelationType.ParentStory)
            {
                return new RelationBetweenEntries(entry, relatedEntry, relationType);
            }
            var entryAsRelatedSubItem = relatedEntry.Related.AllRelatedPositions.First(e => e.MalId == entry.Id);
            var targetToSourceRelation = entryAsRelatedSubItem.RelationType;
            var relation = new RelationBetweenEntries(entry, relatedEntry, relationType, targetToSourceRelation);
            return relation;
        }
    }
}
