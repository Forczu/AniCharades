using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.API.Algorithms.Franchise;
using AniCharades.API.Algorithms.SeriesAssembler;
using AniCharades.API.Logic.Interfaces;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;

namespace AniCharades.API.Logic.Implementation
{
    public class CharadesCompositionService : ICharadesCompositionService
    {
        private readonly IMyAnimeListService myAnimeListService;
        private readonly ISeriesRepository seriesRepository;
        private readonly AnimeSeriesAssembler animeAssembler;
        private readonly IFranchiseCreator franchiseCreator;
        private static readonly object obj = new object();

        public CharadesCompositionService(IMyAnimeListService myAnimeListService, ISeriesRepository seriesRepository, AnimeSeriesAssembler animeAssembler,
            IFranchiseCreator franchiseCreator)
        {
            this.myAnimeListService = myAnimeListService;
            this.seriesRepository = seriesRepository;
            this.animeAssembler = animeAssembler;
            this.franchiseCreator = franchiseCreator;
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
                    var series = animeAssembler.Assembly(malId);
                    existingCharades = currentCharades
                        .FirstOrDefault(c => c.Series.AnimePositions
                            .Any(a => series
                                .Any(s => s.Id == a.MalId && a.MalId != malId)));
                    if (existingCharades == null)
                    {
                        var franchise = franchiseCreator.Create(series);
                        currentCharades.Add(new CharadesEntry() { Series = franchise, KnownBy = { username } });
                    }
                    else
                    {
                        existingCharades.Series.AnimePositions.Add(new AnimeEntry() { MalId = malId, Series = existingCharades.Series });
                    }
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
