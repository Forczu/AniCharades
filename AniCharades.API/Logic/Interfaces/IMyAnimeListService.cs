using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Logic.Interfaces
{
    public interface IMyAnimeListService
    {
        Task<IList<AnimeListEntry>> GetAnimeList(string username);

        UserMangaList GetMangaList(string username);
    }
}
