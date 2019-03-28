using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JikanDotNet;

namespace AniCharades.Algorithms.MyAnimeList.MangaList
{
    public class MangaListExtractor : AbstractListExtractor<MangaListEntry>, IMangaListExtractor
    {
        private readonly IJikan jikan;

        public MangaListExtractor(IJikan jikan)
        {
            this.jikan = jikan;
        }

        public async Task<ICollection<MangaListEntry>> GetFullList(string username)
        {
            return await GetList(username);
        }

        protected override async Task<ICollection<MangaListEntry>> GetUserListPage(string username, int page)
        {
            var result = await jikan.GetUserMangaList(username, UserMangaListExtension.All, page);
            return result?.Manga;
        }
    }
}
