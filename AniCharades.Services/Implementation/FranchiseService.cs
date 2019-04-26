using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Algorithms.Franchise;
using AniCharades.Common.Extensions;
using AniCharades.Common.Utils;
using AniCharades.Contracts.Enums;
using AniCharades.Data.Models;
using AniCharades.Services.Franchise;
using AniCharades.Services.Franchise.Providers;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AniCharades.Services.Implementation
{
    public class FranchiseService : IFranchiseService
    {
        private readonly FranchiseAssembler assembler;
        private readonly IEntryProviderFactory providerFactory;

        public FranchiseService(IEntryProviderFactory providerFactory, FranchiseAssembler assembler)
        {
            this.providerFactory = providerFactory;
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
            return CreateFranchise(animes.Cast<IEntryInstance>().ToArray(), new List<IEntryInstance>());
        }

        public SeriesEntry Create(ICollection<JikanMangaAdapter> mangas)
        {
            if (CollectionUtils.IsCollectionNullOrEmpty(mangas))
                return null;
            return CreateFranchise(new List<IEntryInstance>(), mangas.Cast<IEntryInstance>().ToArray());
        }

        public SeriesEntry CreateFromAnime(long id, AdaptationIncluding withdaptations = AdaptationIncluding.None)
        {
            var provider = providerFactory.Get(EntrySource.Anime);
            var animeEntries = GetFranchiseEntries(id, provider);
            if (animeEntries.Count == 0)
                return null;
            switch (withdaptations)
            {
                case AdaptationIncluding.OnlyFromEntries:
                    var mangaEntries = GetEntriesAdaptations(animeEntries, EntrySource.Manga);
                    return CreateFranchise(animeEntries, mangaEntries);
                case AdaptationIncluding.All:
                case AdaptationIncluding.OnlyKnownInOthers:
                    mangaEntries = GetAllAdaptations(animeEntries, EntrySource.Manga);
                    return CreateFranchise(animeEntries, mangaEntries);
                case AdaptationIncluding.None:
                default:
                    return CreateFranchise(animeEntries, new List<IEntryInstance>());
            }
        }

        public SeriesEntry CreateFromManga(long id, AdaptationIncluding withdaptations = AdaptationIncluding.None)
        {
            var provider = providerFactory.Get(EntrySource.Manga);
            var mangaEntries = GetFranchiseEntries(id, provider);
            if (mangaEntries.Count == 0)
                return null;
            switch (withdaptations)
            {
                case AdaptationIncluding.OnlyFromEntries:
                    var animeEntries = GetEntriesAdaptations(mangaEntries, EntrySource.Anime);
                    return CreateFranchise(animeEntries, mangaEntries);
                case AdaptationIncluding.All:
                case AdaptationIncluding.OnlyKnownInOthers:
                    animeEntries = GetAllAdaptations(mangaEntries, EntrySource.Anime);
                    return CreateFranchise(animeEntries, mangaEntries);
                case AdaptationIncluding.None:
                default:
                    return CreateFranchise(new List<IEntryInstance>(), mangaEntries);
            }
        }

        private ICollection<IEntryInstance> GetFranchiseEntries(long id, IEntryProvider provider)
        {
            var relations = assembler.Assembly(id, provider);
            if (relations.Count == 0)
                return new List<IEntryInstance>();
            var validEntries = GetEntries(relations);
            return validEntries;
        }

        private ICollection<IEntryInstance> GetAllAdaptations(ICollection<IEntryInstance> animeEntries, EntrySource source)
        {
            ICollection<IEntryInstance> entries = new List<IEntryInstance>();
            var adaptationsFromEntries = GetEntriesAdaptations(animeEntries, source);
            var provider = providerFactory.Get(source);
            foreach (var adaptation in adaptationsFromEntries)
            {
                var nextAdaptations = GetFranchiseEntries(adaptation.Id, provider);
                entries.AddRange(nextAdaptations);
            }
            entries = entries.Distinct().ToList();
            return entries;
        }

        private ICollection<IEntryInstance> GetEntriesAdaptations(ICollection<IEntryInstance> entries, EntrySource source)
        {
            var adaptations = entries
                .Where(a => a.Related.Adaptations != null && a.Related.Adaptations.Count != 0)
                .SelectMany(a => a.Related.Adaptations.Select(ad => ad.MalId))
                .Distinct().ToArray();
            var provider = providerFactory.Get(source);
            var adaptationEntries = adaptations.Select(a => provider.Get(a)).Where(a => a != null).ToList();
            return adaptationEntries;
        }

        private SeriesEntry CreateFranchise(ICollection<IEntryInstance> animes, ICollection<IEntryInstance> mangas)
        {
            var entries = animes;
            if (CollectionUtils.IsCollectionNullOrEmpty(animes))
                entries = mangas;
            var mainEntry = MainEntryFinder.GetMainEntry(entries);
            var allEntries = new List<IEntryInstance>();
            allEntries.AddRange(animes);
            allEntries.AddRange(mangas);
            var mainTitle = GetMainTitle(allEntries, mainEntry);
            var series = new SeriesEntry();
            series.AnimePositions.AddRange(animes.Select(e => new AnimeEntry() { MalId = e.Id, Title = e.Title, Series = series }));
            series.MangaPositions.AddRange(mangas.Select(e => new MangaEntry() { MalId = e.Id, Title = e.Title, Series = series }));
            series.ImageUrl = mainEntry.ImageUrl;
            series.Title = mainTitle;
            series.Translation = new Translation()
            {
                Japanese = mainTitle
            };
            bool isTranslationEmpty = string.IsNullOrEmpty(mainEntry.Translation);
            if (!isTranslationEmpty)
            {
                bool isTranslationEqualTitle = mainEntry.Translation.ToLower().Equals(mainTitle.ToLower());
                if (!isTranslationEqualTitle)
                {
                    series.Translation.EnglishOfficial = mainEntry.Translation;
                }
            }
            if (mainEntry.Synonyms != null && mainEntry.Synonyms.Count != 0)
            {
                var synonyms = string.Join(", ", mainEntry.Synonyms);
                if (!synonyms.ToLower().Equals(mainTitle.ToLower()))
                    series.Translation.EnglishLiteral = string.Join(", ", mainEntry.Synonyms);
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
                .Where(e => e.Type.NotIn("Music", "ONA", "One-shot"))
                .ToArray();
            if (CollectionUtils.IsCollectionNullOrEmpty(importantEntries))
                return new MainTitleFinder().GetMainTitle(entries, mainEntry);
            return new MainTitleFinder().GetMainTitle(importantEntries, mainEntry);
        }
    }
}
