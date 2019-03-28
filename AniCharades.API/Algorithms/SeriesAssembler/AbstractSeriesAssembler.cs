using AniCharades.Adapters.Interfaces;
using AniCharades.API.Algorithms.SeriesAssembler.DataStructures;
using AniCharades.API.Logic.Interfaces;
using AniCharades.Data.Enumerations;
using AniCharades.Repositories.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.SeriesAssembler
{
    public abstract class AbstractSeriesAssembler
    {
        private IList<IEntryInstance> series = new List<IEntryInstance>();
        private IList<IEntryInstance> rejected = new List<IEntryInstance>();
        private Stack<IEntryInstance> entriesToCheckTheRelations = new Stack<IEntryInstance>();

        private readonly IRelationService relationService;

        public AbstractSeriesAssembler(IRelationService relationService)
        {
            this.relationService = relationService;
        }

        public ICollection<IEntryInstance> Assembly(long entryId)
        {
            ResetCollections();
            AddEntryToCheckTheRelations(GetEntry(entryId));
            while (!IsStackEmpty())
            {
                ResolveTopEntry();
            }
            return series;
        }

        protected void ResetCollections()
        {
            series.Clear();
            rejected.Clear();
            entriesToCheckTheRelations.Clear();
        }

        protected void AddEntryToCheckTheRelations(IEntryInstance entry)
        {
            entriesToCheckTheRelations.Push(entry);
        }

        protected abstract IEntryInstance GetEntry(long entryId);

        private bool IsStackEmpty()
        {
            return entriesToCheckTheRelations.Count == 0;
        }

        private void ResolveTopEntry()
        {
            var currentEntry = entriesToCheckTheRelations.Pop();
            if (currentEntry == null)
                return;
            series.Add(currentEntry);
            var relations = GetRelations(currentEntry);
            if (IsCollectionEmpty(relations))
                return;
            StackRelatedEntriesIfTheyAreNew(relations);
        }

        protected ICollection<RelationBetweenEntries> GetRelations(IEntryInstance entry)
        {
            var allRelatedToEntry = entry.Related.AllRelatedPositions;
            if (IsCollectionEmpty(allRelatedToEntry))
                return null;
            var relations = new List<RelationBetweenEntries>();
            var filteredEntries = allRelatedToEntry.Where(r => CanEntryBeAddedToSeries(r.MalId));
            foreach (var subItem in filteredEntries)
            {
                var relatedEntry = GetEntry(subItem.MalId);
                if (relatedEntry != null)
                {
                    var relation = new RelationBetweenEntries(entry, relatedEntry, subItem.RelationType);
                    relations.Add(relation);
                }
            }
            return relations;
        }

        protected bool IsCollectionEmpty<K>(ICollection<K> collection)
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

        protected bool CanEntryBeAddedToSeries(IEntryInstance entry)
        {
            if (series.Contains(entry))
                return false;
            if (rejected.Contains(entry))
                return false;
            if (entriesToCheckTheRelations.Contains(entry))
                return false;
            return true;
        }

        protected bool CanEntryBeAddedToSeries(long entryId)
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
