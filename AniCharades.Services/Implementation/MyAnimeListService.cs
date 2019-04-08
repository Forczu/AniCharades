using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Algorithms.MyAnimeList.AnimeList;
using AniCharades.Algorithms.MyAnimeList.MangaList;
using AniCharades.Services.Interfaces;
using JikanDotNet;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<ICollection<IListEntry>> GetMergedAnimeLists(ICollection<string> usernames)
        {
            var mergedList = new List<IListEntry>();
            foreach (var username in usernames)
            {
                var animeList = await GetAnimeList(username);
                foreach (var animeListEntry in animeList)
                {
                    var animeListEntryAdapter = mergedList.FirstOrDefault(e => e.Id == animeListEntry.MalId);
                    if (animeListEntryAdapter != null)
                    {
                        animeListEntryAdapter.AddUser(username);
                    }
                    else
                    {
                        animeListEntryAdapter = new JikanAnimeListEntryAdapter(animeListEntry, username);
                        mergedList.Add(animeListEntryAdapter);
                    }
                }
            }
            return mergedList;
        }

        public async Task<ICollection<IListEntry>> GetMergedMangaLists(ICollection<string> usernames)
        {
            var mergedList = new List<IListEntry>();
            foreach (var username in usernames)
            {
                var mangaList = await GetMangaList(username);
                foreach (var mangaListEntry in mangaList)
                {
                    var mangaListEntryAdapter = mergedList.FirstOrDefault(e => e.Id == mangaListEntry.MalId);
                    if (mangaListEntryAdapter != null)
                    {
                        mangaListEntryAdapter.AddUser(username);
                    }
                    else
                    {
                        mangaListEntryAdapter = new JikanMangaListEntryAdapter(mangaListEntry, username);
                        mergedList.Add(mangaListEntryAdapter);
                    }
                }
            }
            return mergedList;
        }
    }
}
