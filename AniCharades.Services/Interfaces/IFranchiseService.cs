using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Services.Interfaces
{
    public interface IFranchiseService
    {
        SeriesEntry Create(ICollection<IEntryInstance> animes, ICollection<IEntryInstance> mangas);

        SeriesEntry Create(ICollection<JikanAnimeAdapter> animes);

        SeriesEntry Create(ICollection<JikanMangaAdapter> mangas);
    }
}
