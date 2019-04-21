using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Contracts.Enums;
using AniCharades.Data.Models;
using System.Collections.Generic;

namespace AniCharades.Services.Interfaces
{
    public interface IFranchiseService
    {
        SeriesEntry Create(ICollection<IEntryInstance> animes, ICollection<IEntryInstance> mangas);

        SeriesEntry Create(ICollection<JikanAnimeAdapter> animes);

        SeriesEntry Create(ICollection<JikanMangaAdapter> mangas);

        SeriesEntry CreateFromAnime(long id, AdaptationIncluding withdaptations = AdaptationIncluding.None);

        SeriesEntry CreateFromManga(long id, AdaptationIncluding withdaptations = AdaptationIncluding.None);
    }
}
