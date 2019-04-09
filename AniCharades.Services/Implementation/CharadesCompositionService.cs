using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.Adapters.Interfaces;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Franchise;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace AniCharades.Services.Implementation
{
    public class CharadesCompositionService : ICharadesCompositionService
    {
        private static readonly object obj = new object();

        private readonly IMyAnimeListService myAnimeListService;
        private readonly ISeriesRepository seriesRepository;
        private readonly IFranchiseService franchiseService;

        public CharadesCompositionService(IMyAnimeListService myAnimeListService, ISeriesRepository seriesRepository, IFranchiseService franchiseService)
        {
            this.myAnimeListService = myAnimeListService;
            this.seriesRepository = seriesRepository;
            this.franchiseService = franchiseService;
        }

        public async Task<ICollection<CharadesEntry>> GetCharades(ICollection<string> usernames)
        {
            var mergedList = await myAnimeListService.GetMergedAnimeLists(usernames);
            var charades = await CreateCharadesFromAnimeList(mergedList);
            return charades;
        }

        public async Task<ICollection<CharadesEntry>> GetCharades(params string[] usernames)
        {
            return await GetCharades(usernames.ToList());
        }

        private async Task<ICollection<CharadesEntry>> CreateCharadesFromAnimeList(ICollection<IListEntry> mergedList)
        {
            var charades = new List<CharadesEntry>();
            foreach (var entry in mergedList)
            {
                var existingCharades = charades.FirstOrDefault(c => c.Series.AnimePositions.Any(a => a.MalId == entry.Id));
                if (existingCharades != null)
                {
                    foreach (var username in entry.Users)
                    {
                        if (!existingCharades.KnownBy.Contains(username))
                            existingCharades.KnownBy.Add(username);
                        continue;
                    }
                }
                var seriesExistsInDb = await seriesRepository.SeriesExistsByAnimeId(entry.Id);
                if (seriesExistsInDb)
                {
                    var charadesEntry = await GetCharadesFromDatabase(entry.Id, entry.Users);
                    charades.Add(charadesEntry);
                }
                else
                {
                    var franchise = franchiseService.CreateFromAnime(entry.Id);
                    var indirectExistingRelation = GetIndirectExistingRelation(charades, entry.Id, franchise);
                    if (indirectExistingRelation != null)
                    {
                        AddAnimeToCharadesEntry(indirectExistingRelation, entry.Id);
                    }
                    else
                    {
                        charades.Add(new CharadesEntry() { Series = franchise, KnownBy = entry.Users });
                    }
                }
            }
            return charades;
        }

        private async Task<CharadesEntry> GetCharadesFromDatabase(long malId, ICollection<string> usernames)
        {
            var myCharadesEntry = new CharadesEntry()
            {
                Series = await seriesRepository.GetByAnimeId(malId),
                KnownBy = usernames
            };
            return myCharadesEntry;
        }

        private void AddAnimeToCharadesEntry(CharadesEntry charadesEntry, long malId)
        {
            charadesEntry.Series.AnimePositions.Add(new AnimeEntry() { MalId = malId, Series = charadesEntry.Series });
        }

        private CharadesEntry GetIndirectExistingRelation(ICollection<CharadesEntry> charades, long malId, SeriesEntry franchise)
        {
            var indirectExistingRelation = charades
                .FirstOrDefault(c => c.Series.AnimePositions
                    .Any(a => franchise.AnimePositions
                        .Any(f => f.MalId == a.MalId && a.MalId != malId)));
            return indirectExistingRelation;
        }
    }
}
