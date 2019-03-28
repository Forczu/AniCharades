using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.API.Algorithms.SeriesAssembler;
using AniCharades.API.Algorithms.SeriesAssembler.DataStructures;
using AniCharades.API.Logic.Interfaces;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Logic.Implementation
{
    public class RelationService : IRelationService
    {
        private readonly IJikan jikan;
        private readonly IRelationCriteriaRepository relationCriteriaRepository;

        public RelationService(IJikan jikan, IRelationCriteriaRepository relationCriteriaRepository)
        {
            this.jikan = jikan;
            this.relationCriteriaRepository = relationCriteriaRepository;
        }

        public ICollection<long> GetAnimeRelations(long malId)
        {
            var anime = jikan.GetAnime(malId).Result;
            var animeAdapter = new JikanAnimeAdapter(anime);
            var filteredRelations = FilterRelations(animeAdapter);
            return filteredRelations;
        }

        public bool IsRelationValid<T>(RelationBetweenEntries<T> relation) where T : IEntryInstance
        {
            var relationCriteria = relationCriteriaRepository.Get(relation.SourceEntry.Title, relation.Type).Result;
            var relationStrategy = RelationFactory.Instance.Create(relationCriteria.Strategy);
            var areEqual = relationStrategy.AreRelated(relation.SourceEntry, relation.TargetEntry);
            return areEqual;
        }

        private ICollection<long> FilterRelations(JikanAnimeAdapter source)
        {
            var relations = source.Related.AllRelatedPositions;
            var fullRelations = relations.Select(r =>
                new RelationBetweenEntries<JikanAnimeAdapter>(
                    source, new JikanAnimeAdapter(jikan.GetAnime(r.MalId).Result), r.RelationType));
            var filteredRelations = fullRelations.Where(r => IsRelationValid(r)).Select(r => r.TargetEntry.Id).ToArray();
            return filteredRelations;
        }
    }
}
