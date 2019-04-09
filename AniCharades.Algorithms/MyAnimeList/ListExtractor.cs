using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Algorithms.MyAnimeList
{
    public class ListExtractor : IListExtractor
    {
        private readonly IJikan jikan;

        public ListExtractor(IJikan jikan)
        {
            this.jikan = jikan;
        }

        public async Task<ICollection<AnimeListEntry>> GetFullAnimeList(string username)
        {
            return await GetList(username, GetUserAnimeListPage);
        }

        public async Task<ICollection<MangaListEntry>> GetFullMangaList(string username)
        {
            return await GetList(username, GetUserMangaListPage);
        }

        private async Task<ICollection<AnimeListEntry>> GetUserAnimeListPage(string username, int page)
        {
            var result = await jikan.GetUserAnimeList(username, UserAnimeListExtension.All, page);
            return result?.Anime;
        }

        private async Task<ICollection<MangaListEntry>> GetUserMangaListPage(string username, int page)
        {
            var result = await jikan.GetUserMangaList(username, UserMangaListExtension.All, page);
            return result?.Manga;
        }

        private async Task<ICollection<T>> GetList<T>(string username, Func<string, int, Task<ICollection<T>>> provider)
        {
            var entries = new List<T>();
            var pageIndex = 1;
            while (true)
            {
                var nextListPage = await provider(username, pageIndex);
                if (!ListPageExists(nextListPage))
                    break;
                entries.AddRange(nextListPage);
                pageIndex++;
            }
            return entries;
        }

        private bool ListPageExists<T>(ICollection<T> page)
        {
            return page != null && page.Count > 0;
        }
    }
}
