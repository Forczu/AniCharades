using AniCharades.Algorithms.MyAnimeList.AnimeList;
using AniCharades.Algorithms.MyAnimeList.MangaList;
using AniCharades.Services.Interfaces;
using JikanDotNet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AniCharades.Services.Implementation
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
