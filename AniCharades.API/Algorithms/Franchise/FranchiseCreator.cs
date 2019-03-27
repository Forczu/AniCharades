using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Common.Utils;
using AniCharades.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.Franchise
{
    public class FranchiseCreator : IFranchiseCreator
    {
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
    }
}
