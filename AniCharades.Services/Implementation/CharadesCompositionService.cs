using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Contracts.Charades;
using AniCharades.Contracts.Enums;
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
        private readonly IMyAnimeListService myAnimeListService;
        private readonly ISeriesRepository seriesRepository;
        private readonly IFranchiseService franchiseService;

        private ICollection<string> usernames;
        private Queue<EntrySource> sources = new Queue<EntrySource>();
        private Queue<EntrySource> otherSources = new Queue<EntrySource>();

        private IList<IListEntry> currentMergedList = new List<IListEntry>();
        private EntrySource currentSource;
        private int nextEntryIndex = 0;

        private ICollection<CharadesEntry> charades;

        public CharadesCompositionService(IMyAnimeListService myAnimeListService, ISeriesRepository seriesRepository, IFranchiseService franchiseService)
        {
            this.myAnimeListService = myAnimeListService;
            this.seriesRepository = seriesRepository;
            this.franchiseService = franchiseService;
        }

        public async Task<ICollection<CharadesEntry>> GetCharades(GetCharadesCriteria criteria)
        {
            StartComposing(criteria);
            while(IsFinished())
            {
                await MakeNextCharadesEntry();
            }
            return charades;
        }

        public void StartComposing(ICollection<IListEntry> entires)
        {
            currentMergedList = entires.ToList();
            Reset();
        }

        public void StartComposing(GetCharadesCriteria criteria)
        {
            Reset();
            usernames = criteria.Usernames;
            sources.AddRange(criteria.Sources);
            if (criteria.IncludeKnownAdaptations && sources.Count != 0)
            {
                var allSources = Enum.GetValues(typeof(EntrySource)).Cast<EntrySource>();
                var isAllDone = criteria.Sources.Count == allSources.Count();
                if (!isAllDone)
                {
                    var otherSources = allSources.Except(criteria.Sources);
                    this.otherSources.AddRange(otherSources);
                }
            }
        }

        public ICollection<CharadesEntry> GetFinishedCharades()
        {
            return charades;
        }

        public bool IsFinished()
        {
            return !(sources.Count != 0 || otherSources.Count != 0 || nextEntryIndex != currentMergedList.Count);
        }

        public async Task<CharadesEntry> MakeNextCharadesEntry()
        {
            bool shouldGoToNextSource = nextEntryIndex == currentMergedList.Count;
            if (shouldGoToNextSource)
            {
                nextEntryIndex = 0;
                if (sources.Count != 0)
                {
                    currentSource = sources.Dequeue();
                    await GetCurrentMergedListFromSource();
                }
                else if (otherSources.Count != 0)
                {
                    currentSource = otherSources.Dequeue();
                    await GetCurrentMergedListFromSource();
                }
                else
                {
                    return null;
                }
            }
            var entry = currentMergedList[nextEntryIndex++];
            var nextCharades = await CreateNextCharades(entry);
            return nextCharades;
        }

        private async Task GetCurrentMergedListFromSource()
        {
            switch (currentSource)
            {
                case EntrySource.Anime:
                    currentMergedList = (await myAnimeListService.GetMergedAnimeLists(usernames)).ToList();
                    break;
                case EntrySource.Manga:
                    break;
                default:
                    break;
            }
        }

        private async Task<CharadesEntry> CreateNextCharades(IListEntry entry)
        {
            CharadesEntry nextCharades = null;
            switch (currentSource)
            {
                case EntrySource.Anime:

                    nextCharades = await CreateNextCharadesEntryFromAnime(entry);
                    break;
                case EntrySource.Manga:
                    break;
                default:
                    break;
            }
            return nextCharades;
        }

        private void Reset()
        {
            charades = new List<CharadesEntry>();
            sources.Clear();
            otherSources.Clear();
            nextEntryIndex = 0;
        }

        private async Task CreateCharadesFromList(ICollection<IListEntry> mergedList)
        {
            Reset();
            currentMergedList = mergedList.ToList();
            foreach (var entry in mergedList)
            {
                await CreateNextCharadesEntryFromAnime(entry);
            }
        }

        private async Task<CharadesEntry> CreateNextCharadesEntryFromAnime(IListEntry entry)
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
                var existingEntry = currentMergedList.FirstOrDefault(x => x.Id == id);
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
