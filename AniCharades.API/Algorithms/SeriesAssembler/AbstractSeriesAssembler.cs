using AniCharades.Adapters.Interfaces;
using AniCharades.API.Algorithms.SeriesAssembler.DataStructures;
using AniCharades.Data.Enumerations;
using AniCharades.Repositories.Interfaces;
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

        private readonly IRelationCriteriaRepository relationCriteriaRepository;

        public AbstractSeriesAssembler(IRelationCriteriaRepository relationCriteriaRepository)
        {
            this.relationCriteriaRepository = relationCriteriaRepository;
        }

        public ICollection<T> Assembly(long entryId)
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
            var relations = GetRelations(currentEntry);
            if (IsCollectionEmpty(relations))
                return;
            StackRelatedEntriesIfTheyAreNew(relations);
        }

        protected ICollection<RelationBetweenEntries<T>> GetRelations(T entry)
        {
            var allRelatedToEntry = entry.Related.AllRelatedPositions;
            if (IsCollectionEmpty(allRelatedToEntry))
                return null;
            var relations = new List<RelationBetweenEntries<T>>();
            var filteredEntries = allRelatedToEntry.Where(r => CanEntryBeAddedToSeries(r.MalId));
            foreach (var subItem in filteredEntries)
            {
                var relatedEntry = GetEntry(subItem.MalId);
                if (relatedEntry != null)
                {
                    var relation = new RelationBetweenEntries<T>(entry, relatedEntry, subItem.RelationType);
                    relations.Add(relation);
                }
            }
            return relations;
        }

        protected bool IsCollectionEmpty<K>(ICollection<K> collection)
        {
            return collection == null || collection.Count == 0;
        }

        private void StackRelatedEntriesIfTheyAreNew(ICollection<RelationBetweenEntries<T>> relations)
        {
            foreach (var relation in relations.Where(e => CanEntryBeAddedToSeries(e.TargetEntry)))
            {
                var isRelationValid = CheckIfRelationIsValidForSeries(relation);
                if (isRelationValid)
                    AddEntryToCheckTheRelations(relation.TargetEntry);
                else
                    RejectEntry(relation.TargetEntry);
            }
        }

        protected bool CanEntryBeAddedToSeries(T entry)
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

        private bool CheckIfRelationIsValidForSeries(RelationBetweenEntries<T> relation)
        {
            var relationCriteria = relationCriteriaRepository.Get(relation.SourceEntry.Title, relation.Type).Result;
            var relationStrategy = RelationFactory.Instance.Create(relationCriteria.Strategy);
            var areEqual = relationStrategy.AreRelated(relation.SourceEntry, relation.TargetEntry);
            return areEqual;
        }

        private void RejectEntry(T entry)
        {
            if (!rejected.Contains(entry))
                rejected.Add(entry);
        }
    }
}
