using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Algorithms.Franchise;
using AniCharades.Common.Utils;
using AniCharades.Data.Models;
using AniCharades.Services.Franchise;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using JikanDotNet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Services.Implementation
{
    public class FranchiseService : IFranchiseService
    {
        private readonly FranchiseAssembler assembler = new FranchiseAssembler();
        private readonly IServiceProvider serviceProvider;
        private readonly IRelationService relationService;

        public FranchiseService(IServiceProvider serviceProvider, IRelationService relationService)
        {
            this.serviceProvider = serviceProvider;
            this.relationService = relationService;
        }

        public SeriesEntry Create(ICollection<IEntryInstance> animes, ICollection<IEntryInstance> mangas)
        {
            if (CollectionUtils.IsCollectionNullOrEmpty(animes) && CollectionUtils.IsCollectionNullOrEmpty(mangas))
                return null;
            return CreateFranchise(animes, mangas);
        }

        public SeriesEntry Create(ICollection<JikanAnimeAdapter> animes)
        {
            if (CollectionUtils.IsCollectionNullOrEmpty(animes))
                return null;
            return CreateFranchise(animes.Cast<IEntryInstance>().ToArray(), null);
        }

        public SeriesEntry Create(ICollection<JikanMangaAdapter> mangas)
        {
            if (CollectionUtils.IsCollectionNullOrEmpty(mangas))
                return null;
            return CreateFranchise(null, mangas.Cast<IEntryInstance>().ToArray());
        }

        public SeriesEntry CreateFromAnime(long id)
        {
            var provider = serviceProvider.GetRequiredService<JikanAnimeProvider>();
            var relations = assembler.Assembly(id, provider);
            var validEntries = GetValidEntries(relations);
            var franchise = CreateFranchise(validEntries, null);
            return franchise;
        }

        private SeriesEntry CreateFranchise(ICollection<IEntryInstance> animes, ICollection<IEntryInstance> mangas)
        {
            var mainEntries = animes;
            if (CollectionUtils.IsCollectionNullOrEmpty(animes))
                mainEntries = mangas;
            var mainEntry = MainEntryFinder.GetMainEntry(mainEntries);
            var mainTitle = MainTitleFinder.GetMainTitle(mainEntry, mainEntries);
            var series = new SeriesEntry();
            series.AnimePositions = animes?.Select(e => new AnimeEntry() { MalId = e.Id, Series = series }).ToArray();
            series.MangaPositions = mangas?.Select(e => new MangaEntry() { MalId = e.Id, Series = series }).ToArray();
            series.ImageUrl = mainEntry.ImageUrl;
            series.Title = mainTitle;
            series.Translations = new[] { mainEntry.Translation };
            return series;
        }

        private ICollection<IEntryInstance> GetValidEntries(ICollection<RelationBetweenEntries> relations)
        {
            var validEntries = relations
                .Where(r => relationService.IsRelationValid(r))
                .Select(r => r.TargetEntry)
                .ToList();
            validEntries.Add(relations.First().SourceEntry);
            return validEntries;
        }
    }
}
