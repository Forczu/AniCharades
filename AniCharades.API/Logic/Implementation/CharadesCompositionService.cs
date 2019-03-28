using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.API.Algorithms.Franchise;
using AniCharades.API.Algorithms.SeriesAssembler;
using AniCharades.API.Algorithms.SeriesAssembler.Providers;
using AniCharades.API.Logic.Interfaces;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AniCharades.API.Logic.Implementation
{
    public class CharadesCompositionService : ICharadesCompositionService
    {
        private static readonly object obj = new object();

        private readonly IMyAnimeListService myAnimeListService;
        private readonly ISeriesRepository seriesRepository;
        private readonly SeriesAssembler seriesAssembler;
        private readonly IFranchiseCreator franchiseCreator;
        private readonly IServiceProvider serviceProvider;

        public CharadesCompositionService(IMyAnimeListService myAnimeListService, ISeriesRepository seriesRepository, SeriesAssembler seriesAssembler,
            IFranchiseCreator franchiseCreator, IServiceProvider serviceProvider)
        {
            this.myAnimeListService = myAnimeListService;
            this.seriesRepository = seriesRepository;
            this.seriesAssembler = seriesAssembler;
            this.franchiseCreator = franchiseCreator;
            this.serviceProvider = serviceProvider;
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
                    var series = seriesAssembler.Assembly(malId, serviceProvider.GetRequiredService<JikanAnimeProvider>());
                    var indirectExistingRelation = GetIndirectExistingRelation(currentCharades, malId, series);
                    if (indirectExistingRelation != null)
                    {
                        AddAnimeToCharadesEntry(indirectExistingRelation, malId);
                        return;
                    }
                    else
                    {
                        var franchise = franchiseCreator.Create(series, null);
                        currentCharades.Add(new CharadesEntry() { Series = franchise, KnownBy = { username } });
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

        private void AddAnimeToCharadesEntry(CharadesEntry charadesEntry, long malId)
        {
            charadesEntry.Series.AnimePositions.Add(new AnimeEntry() { MalId = malId, Series = charadesEntry.Series });
        }

        private CharadesEntry GetIndirectExistingRelation(ConcurrentBag<CharadesEntry> charades, long malId, ICollection<IEntryInstance> series)
        {
            var indirectExistingRelation = charades
                .FirstOrDefault(c => c.Series.AnimePositions
                    .Any(a => series
                        .Any(s => s.Id == a.MalId && a.MalId != malId)));
            return indirectExistingRelation;
        }
    }
}
