using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JikanDotNet;

namespace AniCharades.Algorithms.MyAnimeList.AnimeList
{
    public class AnimeListExtractor : AbstractListExtractor<AnimeListEntry>, IAnimeListExtractor
    {
        private readonly IJikan jikan;

        public AnimeListExtractor(IJikan jikan)
        {
            this.jikan = jikan;
        }

        public async Task<ICollection<AnimeListEntry>> GetFullList(string username)
        {
            return await GetList(username);
        }

        protected override async Task<ICollection<AnimeListEntry>> GetUserListPage(string username, int page)
        {
            var result = await jikan.GetUserAnimeList(username, UserAnimeListExtension.All, page);
            return result?.Anime;
        }
    }
}
