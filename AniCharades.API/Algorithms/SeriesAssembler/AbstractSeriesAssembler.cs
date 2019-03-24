using AniCharades.Adapters.Interfaces;
using AniCharades.API.Algorithms.SeriesAssembler.DataStructures;
using AniCharades.Data.Enumerations;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.SeriesAssembler
{
    public abstract class AbstractSeriesAssembler<T> where T : IEntryInstance
    {
        private IList<T> series = new List<T>();
        private IList<T> rejected = new List<T>();
        private Stack<T> entriesToCheckTheRelations = new Stack<T>();

        public void Assembly(long entryId)
        {
            ResetCollections();
            AddEntryToCheckTheRelations(GetEntry(entryId));
            while (!IsStackEmpty())
            {
                ResolveTopEntry();
            }
        }

        protected void ResetCollections()
        {
            series.Clear();
            rejected.Clear();
            entriesToCheckTheRelations.Clear();
        }

        protected void AddEntryToCheckTheRelations(T entry)
        {
            entriesToCheckTheRelations.Push(entry);
        }

        protected abstract T GetEntry(long entryId);

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
            var relatedEntries = GetRelatedEntries(currentEntry);
            if (IsCollectionEmpty(relatedEntries))
                return;
            StackRelatedEntriesIfTheyAreNew(currentEntry, relatedEntries);
        }

        protected ICollection<EntryInstanceRelated<T>> GetRelatedEntries(T entry)
        {
            var allRelatedToEntry = entry.Related.AllRelatedPositions;
            if (IsCollectionEmpty(allRelatedToEntry))
                return null;
            var newEntries = new List<EntryInstanceRelated<T>>();
            var filteredEntries = allRelatedToEntry.Where(r => CanEntryBeAddedToSeries(r.MalId));
            foreach (var subItem in filteredEntries)
            {
                var relatedEntry = GetEntry(subItem.MalId);
                if (relatedEntry != null)
                {
                    var fullEntry = new EntryInstanceRelated<T>(relatedEntry, subItem.RelationType);
                    newEntries.Add(fullEntry);
                }
            }
            return newEntries;
        }

        protected bool IsCollectionEmpty<K>(ICollection<K> relatedEntries)
        {
            return relatedEntries != null && relatedEntries.Count == 0;
        }

        private void StackRelatedEntriesIfTheyAreNew(T entry, ICollection<EntryInstanceRelated<T>> relatedEntries)
        {
            foreach (var relatedEntry in relatedEntries.Where(e => CanEntryBeAddedToSeries(e.Entry)))
            {
                var isRelationValid = CheckIfRelationIsValidForSeries(entry, relatedEntry.Entry, relatedEntry.RelationType);
                if (isRelationValid)
                    AddEntryToCheckTheRelations(relatedEntry.Entry);
                else
                    RejectEntry(relatedEntry.Entry);
            }
        }

        protected bool CanEntryBeAddedToSeries(T entry)
        {
            if (series.Contains(entry))
                return false;
            if (rejected.Contains(entry))
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

        private bool CheckIfRelationIsValidForSeries(T sourceEntry, T relatedEntry, RelationType relationType)
        {
            var relationStrategy = RelationFactory.Create(sourceEntry.Title, relationType);
            var areEqual = relationStrategy.AreEqual(sourceEntry, relatedEntry);
            return areEqual;
        }

        private void RejectEntry(T entry)
        {
            if (!rejected.Contains(entry))
                rejected.Add(entry);
        }
    }
}
