using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
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

        private IList<IListEntry> mergedList;
        private ICollection<CharadesEntry> charades;
        private int nextEntryIndex = 0;

        public CharadesCompositionService(IMyAnimeListService myAnimeListService, ISeriesRepository seriesRepository, IFranchiseService franchiseService)
        {
            this.myAnimeListService = myAnimeListService;
            this.seriesRepository = seriesRepository;
            this.franchiseService = franchiseService;
        }

        public async Task<ICollection<CharadesEntry>> GetCharades(ICollection<string> usernames)
        {
            var mergedList = await myAnimeListService.GetMergedAnimeLists(usernames);
            await CreateCharadesFromAnimeList(mergedList);
            return charades;
        }

        public async Task<ICollection<CharadesEntry>> GetCharades(params string[] usernames)
        {
            return await GetCharades(usernames.ToList());
        }

        public async Task StartComposing(ICollection<string> usernames)
        {
            var mergedAnimeLists = await myAnimeListService.GetMergedAnimeLists(usernames);
            mergedList = mergedAnimeLists.ToList();
            charades = new List<CharadesEntry>();
        }

        public int GetMergedPositionsCount()
        {
            return mergedList.Count;
        }

        public async Task<CharadesEntry> MakeNextCharadesEntry()
        {
            var entry = mergedList[nextEntryIndex++];
            var nextCharades = await CreateNextCharadesEntryIfDoesntExist(entry);
            return nextCharades;
        }

        private async Task CreateCharadesFromAnimeList(ICollection<IListEntry> mergedList)
        {
            charades = new List<CharadesEntry>();
            foreach (var entry in mergedList)
            {
                await CreateNextCharadesEntryIfDoesntExist(entry);
            }
        }

        private async Task<CharadesEntry> CreateNextCharadesEntryIfDoesntExist(IListEntry entry)
        {
            var seriesExistsInDb = await seriesRepository.SeriesExistsByAnimeId(entry.Id);
            if (seriesExistsInDb)
            {
                var nextCharades = await GetCharadesFromDatabase(entry.Id);
                charades.Add(nextCharades);
                return nextCharades;
            }
            var franchise = franchiseService.CreateFromAnime(entry.Id);
            var indirectExistingRelation = GetIndirectExistingRelation(charades, entry.Id, franchise);
            if (indirectExistingRelation != null)
            {
                AddAnimeToCharadesEntry(indirectExistingRelation, entry);
                return null;
            }
            var charadesEntry = new CharadesEntry() { Series = franchise, KnownBy = GetAllUsersForFranchise(franchise) };
            charades.Add(charadesEntry);
            return charadesEntry;
        }

        private async Task<CharadesEntry> GetCharadesFromDatabase(long malId)
        {
            var franchise = await seriesRepository.GetByAnimeId(malId);
            var myCharadesEntry = new CharadesEntry()
            {
                Series = franchise,
                KnownBy = GetAllUsersForFranchise(franchise)
            };
            return myCharadesEntry;
        }

        private ICollection<string> GetAllUsersForFranchise(SeriesEntry franchise)
        {
            var usernames = new List<string>();
            foreach (var id in franchise.AnimePositions.Select(a => a.MalId))
            {
                var existingEntry = mergedList.FirstOrDefault(x => x.Id == id);
                if (existingEntry != null)
                {
                    existingEntry.Users.Where(u => !usernames.Contains(u)).ForEach(u => usernames.Add(u));
                }
            }
            return usernames;
        }

        private void AddAnimeToCharadesEntry(CharadesEntry charadesEntry, IListEntry entry)
        {
            charadesEntry.Series.AnimePositions.Add(new AnimeEntry() { MalId = entry.Id, Title = entry.Title, Series = charadesEntry.Series });
            charadesEntry.KnownBy = GetAllUsersForFranchise(charadesEntry.Series);
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
