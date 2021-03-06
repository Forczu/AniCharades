﻿using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Algorithms.MyAnimeList;
using AniCharades.Services.Interfaces;
using JikanDotNet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Services.Implementation
{
    public class MyAnimeListService : IMyAnimeListService
    {
        private readonly IListExtractor listExtractor;

        public MyAnimeListService(IListExtractor listExtractor)
        {
            this.listExtractor = listExtractor;
        }

        public async Task<ICollection<AnimeListEntry>> GetAnimeList(string username)
        {
            var userAnimeList = await listExtractor.GetFullAnimeList(username);
            return userAnimeList;
        }

        public async Task<ICollection<MangaListEntry>> GetMangaList(string username)
        {
            var userMangaList = await listExtractor.GetFullMangaList(username);
            return userMangaList;
        }

        public async Task<ICollection<IListEntry>> GetMergedAnimeLists(ICollection<string> usernames)
        {
            var mergedList = new List<IListEntry>();
            foreach (var username in usernames)
            {
                var animeList = await GetAnimeList(username);
                foreach (var animeListEntry in animeList.Where(a => a.WatchingStatus != UserAnimeListExtension.PlanToWatch))
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
                foreach (var mangaListEntry in mangaList.Where(m => m.ReadingStatus != UserMangaListExtension.PlanToRead))
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
