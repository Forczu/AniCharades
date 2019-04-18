﻿using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Algorithms.Franchise;
using AniCharades.Common.Extensions;
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
        private readonly FranchiseAssembler assembler;
        private readonly IServiceProvider serviceProvider;

        public FranchiseService(IServiceProvider serviceProvider, FranchiseAssembler assembler)
        {
            this.serviceProvider = serviceProvider;
            this.assembler = assembler;
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
            var provider = serviceProvider.GetService(typeof(JikanAnimeProvider)) as JikanAnimeProvider;
            var relations = assembler.Assembly(id, provider);
            var validEntries = GetEntries(relations);
            var franchise = CreateFranchise(validEntries, null);
            return franchise;
        }

        private SeriesEntry CreateFranchise(ICollection<IEntryInstance> animes, ICollection<IEntryInstance> mangas)
        {
            var entries = animes;
            if (CollectionUtils.IsCollectionNullOrEmpty(animes))
                entries = mangas;
            var mainEntry = MainEntryFinder.GetMainEntry(entries);
            var mainTitle = GetMainTitle(entries, mainEntry);
            var series = new SeriesEntry();
            series.AnimePositions = animes?.Select(e => new AnimeEntry() { MalId = e.Id, Title = e.Title, Series = series }).ToList();
            series.MangaPositions = mangas?.Select(e => new MangaEntry() { MalId = e.Id, Title = e.Title, Series = series }).ToList();
            series.ImageUrl = mainEntry.ImageUrl;
            series.Title = mainTitle;
            series.Translation = new Translation()
            {
                Japanese = mainTitle
            };
            bool isTranslationEmpty = string.IsNullOrEmpty(mainEntry.Translation);
            if (!isTranslationEmpty)
            {
                bool isTranslationEqualTitle = mainEntry.Translation.Equals(mainTitle);
                if (!isTranslationEqualTitle)
                    series.Translation.English = mainEntry.Translation;
            }
            return series;
        }

        private ICollection<IEntryInstance> GetEntries(ICollection<RelationBetweenEntries> relations)
        {
            var validEntries = relations
                .Where(r => r.TargetEntry != null)
                .Select(r => r.TargetEntry)
                .ToList();
            validEntries.Add(relations.First().SourceEntry);
            return validEntries;
        }

        private static string GetMainTitle(ICollection<IEntryInstance> entries, IEntryInstance mainEntry)
        {
            var importantEntries = entries
                .Where(e => e.Type.NotIn("Music", "ONA"))
                .ToArray();
            if (CollectionUtils.IsCollectionNullOrEmpty(importantEntries))
                return new MainTitleFinder().GetMainTitle(entries, mainEntry);
            return new MainTitleFinder().GetMainTitle(importantEntries, mainEntry);
        }
    }
}
