using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Contracts.Charades;
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

        public async Task<ICollection<CharadesEntry>> GetCharades(GetCharadesCriteria criteria)
        {
            var mergedList = await myAnimeListService.GetMergedAnimeLists(criteria.Usernames);
            await CreateCharadesFromAnimeList(mergedList);
            return charades;
        }

        public async Task StartComposing(ICollection<IListEntry> entires)
        {
            mergedList = entires.ToList();
            Reset();
        }

        public async Task StartComposing(ICollection<string> usernames)
        {
            var mergedAnimeLists = await myAnimeListService.GetMergedAnimeLists(usernames);
            mergedList = mergedAnimeLists.ToList();
            Reset();
        }

        public async Task<ICollection<CharadesEntry>> GetFinishedCharades()
        {
            return charades;
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

        private void Reset()
        {
            charades = new List<CharadesEntry>();
            nextEntryIndex = 0;
        }

        private async Task CreateCharadesFromAnimeList(ICollection<IListEntry> mergedList)
        {
            Reset();
            foreach (var entry in mergedList)
            {
                await CreateNextCharadesEntryIfDoesntExist(entry);
            }
        }

        private async Task<CharadesEntry> CreateNextCharadesEntryIfDoesntExist(IListEntry entry)
        {
            var existingCharadesWithEntry = charades.FirstOrDefault(c => c.Series.AnimePositions.Any(a => a.MalId == entry.Id));
            if (existingCharadesWithEntry != null)
            {
                return existingCharadesWithEntry;
            }
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
                return indirectExistingRelation;
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
                    var newUsers = existingEntry.Users.Where(u => !usernames.Contains(u)).ToArray();
                    newUsers.ForEach(u => usernames.Add(u));
                }
            }
            return usernames;
        }

        private void AddAnimeToCharadesEntry(CharadesEntry charadesEntry, IListEntry entry)
        {
            charadesEntry.Series.AnimePositions.Add(new AnimeEntry() { MalId = entry.Id, Title = entry.Title, Series = charadesEntry.Series });
            var newUsers = entry.Users.Where(u => !charadesEntry.KnownBy.Contains(u)).ToArray();
            newUsers.ForEach(u => charadesEntry.KnownBy.Add(u));
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
