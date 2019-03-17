using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.API.Algorithms.MyAnimeList.AnimeList;
using AniCharades.API.Algorithms.MyAnimeList.MangaList;
using AniCharades.API.Logic.Interfaces;
using JikanDotNet;

namespace AniCharades.API.Logic.Implementation
{
    public class MyAnimeListService : IMyAnimeListService
    {
        private readonly IAnimeListExtractor animeListExtractor;
        private readonly IMangaListExtractor mangaListExtractor;

        public MyAnimeListService(IAnimeListExtractor animeListExtractor, IMangaListExtractor mangaListExtractor)
        {
            this.animeListExtractor = animeListExtractor;
            this.mangaListExtractor = mangaListExtractor;
        }

        public async Task<ICollection<AnimeListEntry>> GetAnimeList(string username)
        {
            var userAnimeList = await animeListExtractor.GetFullList(username);
            return userAnimeList;
        }

        public async Task<ICollection<MangaListEntry>> GetMangaList(string username)
        {
            var userMangaList = await mangaListExtractor.GetFullList(username);
            return userMangaList;
        }
    }
}
