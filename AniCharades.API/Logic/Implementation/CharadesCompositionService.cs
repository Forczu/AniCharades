using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.API.Data;
using AniCharades.API.Logic.Interfaces;
using AniCharades.Data.Models;

namespace AniCharades.API.Logic.Implementation
{
    public class CharadesCompositionService : ICharadesCompositionService
    {
        private readonly IMyAnimeListService myAnimeListService;
        private readonly ISeriesRepository seriesRepository;

        private static readonly object obj = new object();

        public CharadesCompositionService(IMyAnimeListService myAnimeListService, ISeriesRepository seriesRepository)
        {
            this.myAnimeListService = myAnimeListService;
            this.seriesRepository = seriesRepository;
        }

        public async Task<ICollection<CharadesEntry>> GetCompositedCharades(IEnumerable<string> usernames)
        {
            ConcurrentBag<CharadesEntry> charades = new ConcurrentBag<CharadesEntry>();
            var tasks = usernames.Select(u => CreateCharadesForSingleUser(charades, u));
            await Task.WhenAll(tasks);
            return charades.ToList();
        }

        private async Task CreateCharadesForSingleUser(ConcurrentBag<CharadesEntry> currentCharades, string username)
        {
            var userAnimeList = await myAnimeListService.GetAnimeList(username);
            userAnimeList = userAnimeList.Where(a => a.WatchingStatus != JikanDotNet.UserAnimeListExtension.PlanToWatch).ToList();
            var tasks = userAnimeList.Select(a => CreateCharadesFromAnimeListEntry(currentCharades, a.MalId, username));
            await Task.WhenAll(tasks);
        }

        private async Task CreateCharadesFromAnimeListEntry(ConcurrentBag<CharadesEntry> currentCharades, long malId, string username)
        {
            var existingCharades = currentCharades.FirstOrDefault(c => c.Series.AnimePositions.Any(a => a.MalId == malId));
            if (existingCharades != null)
            {
                if (!existingCharades.KnownBy.Contains(username))
                    existingCharades.KnownBy.Add(username);
                return;
            }
            lock (obj)
            {
                var seriesExistsInDb = seriesRepository.SeriesExistsByAnimeId(malId).Result;
                if (seriesExistsInDb)
                {
                    var charadesEntry = GetCharadesFromDatabase(malId, username).Result;
                    currentCharades.Add(charadesEntry);
                }
                else
                {
                    // TODO: Scrapping
                }
            }
        }

        private async Task<CharadesEntry> GetCharadesFromDatabase(long malId, string username)
        {
            var myCharadesEntry = new CharadesEntry()
            {
                Series = await seriesRepository.GetByAnimeId(malId),
                KnownBy = new List<string>() { username }
            };
            return myCharadesEntry;
        }
    }
}
