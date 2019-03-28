using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Services.Interfaces
{
    public interface IMyAnimeListService
    {
        Task<ICollection<AnimeListEntry>> GetAnimeList(string username);

        Task<ICollection<MangaListEntry>> GetMangaList(string username);
    }
}
