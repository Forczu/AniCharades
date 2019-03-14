using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.API.Logic.Interfaces;
using JikanDotNet;

namespace AniCharades.API.Logic.Implementation
{
    public class MyAnimeListService : IMyAnimeListService
    {
        private readonly IJikan jikan;

        public MyAnimeListService(IJikan jikan)
        {
            this.jikan = jikan;
        }

        public async Task<IList<AnimeListEntry>> GetAnimeList(string username)
        {
            var userAnimeList = await GetUserAnimeListFull(username, UserAnimeListExtension.All);
            return userAnimeList;
        }

        public UserMangaList GetMangaList(string username)
        {
            return null;
        }

        private async Task<IList<AnimeListEntry>> GetUserAnimeListFull(string username, UserAnimeListExtension filters)
        {
            var animeEntries = new List<AnimeListEntry>();
            var pageIndex = 1;
            while (true)
            {
                var nextAnimeListPage = await GetUserAnimeListPage(username, filters, pageIndex);
                if (!AnimeListPageExists(nextAnimeListPage))
                    break;
                animeEntries.AddRange(nextAnimeListPage.Anime);
                pageIndex++;
            }
            return animeEntries;
        }

        private bool AnimeListPageExists(UserAnimeList page)
        {
            return page != null && page.Anime.Count > 0;
        }

        private async Task<UserAnimeList> GetUserAnimeListPage(string username, UserAnimeListExtension filters, int page)
        {
            var result = await jikan.GetUserAnimeList(username, filters, page);
            return result;
        }
    }
}
