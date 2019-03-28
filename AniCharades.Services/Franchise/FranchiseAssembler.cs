using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AniCharades.Services.Franchise
{
    public class FranchiseAssembler
    {
        private IList<IEntryInstance> series = new List<IEntryInstance>();
        private IList<IEntryInstance> rejected = new List<IEntryInstance>();
        private Stack<IEntryInstance> entriesToCheckTheRelations = new Stack<IEntryInstance>();
        
        private readonly IRelationService relationService;

        public FranchiseAssembler(IRelationService relationService)
        {
            this.relationService = relationService;
        }

        public ICollection<IEntryInstance> Assembly(long entryId, IEntryProvider entryProvider)
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
            rejected.Clear();
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
            series.Add(currentEntry);
            var relations = GetRelations(currentEntry, entryProvider);
            if (IsCollectionEmpty(relations))
                return;
            StackRelatedEntriesIfTheyAreNew(relations);
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
                    var relation = new RelationBetweenEntries(entry, relatedEntry, subItem.RelationType);
                    relations.Add(relation);
                }
            }
            return relations;
        }

        private bool IsCollectionEmpty<K>(ICollection<K> collection)
        {
            return collection == null || collection.Count == 0;
        }

        private void StackRelatedEntriesIfTheyAreNew(ICollection<RelationBetweenEntries> relations)
        {
            foreach (var relation in relations.Where(e => CanEntryBeAddedToSeries(e.TargetEntry)))
            {
                var isRelationValid = relationService.IsRelationValid(relation);
                if (isRelationValid)
                    AddEntryToCheckTheRelations(relation.TargetEntry);
                else
                    RejectEntry(relation.TargetEntry);
            }
        }

        private bool CanEntryBeAddedToSeries(IEntryInstance entry)
        {
            if (series.Contains(entry))
                return false;
            if (rejected.Contains(entry))
                return false;
            if (entriesToCheckTheRelations.Contains(entry))
                return false;
            return true;
        }

        private bool CanEntryBeAddedToSeries(long entryId)
        {
            if (series.Any(e => e.Id == entryId))
                return false;
            if (rejected.Any(e => e.Id == entryId))
                return false;
            return true;
        }

        private void RejectEntry(IEntryInstance entry)
        {
            if (!rejected.Contains(entry))
                rejected.Add(entry);
        }
    }
}
