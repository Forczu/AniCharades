using AniCharades.API.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.SeriesAssembler
{
    public abstract class AbstractSeriesAssembler<T> where T : class
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

        protected abstract T GetEntry(long malId);

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

        protected abstract ICollection<T> GetRelatedEntries(T entry);

        private bool IsCollectionEmpty(ICollection<T> relatedEntries)
        {
            return relatedEntries != null && relatedEntries.Count == 0;
        }

        private void StackRelatedEntriesIfTheyAreNew(T entry, ICollection<T> relatedEntries)
        {
            foreach (var relatedEntry in relatedEntries.Where(e => CanEntryBeAddedToSeries(e)))
            {
                var isRelationValid = CheckIfRelationIsValidForSeries(entry, relatedEntry);
                if (isRelationValid)
                    AddEntryToCheckTheRelations(relatedEntry);
                else
                    RejectEntry(relatedEntry);
            }
        }

        private bool CanEntryBeAddedToSeries(T entry)
        {
            if (series.Contains(entry))
                return false;
            if (rejected.Contains(entry))
                return false;
            return true;
        }

        protected abstract bool CheckIfRelationIsValidForSeries(T sourceEntry, T relatedEntry);

        private void RejectEntry(T entry)
        {
            if (!rejected.Contains(entry))
                rejected.Add(entry);
        }
    }
}
