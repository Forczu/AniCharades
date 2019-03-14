using AniCharades.API.Models.MyAnimeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Logic
{
    public interface IMyAnimeListService
    {
        UserAnimeList GetAnimeList(string username);

        UserMangaList GetMangaList(string username);
    }
}
