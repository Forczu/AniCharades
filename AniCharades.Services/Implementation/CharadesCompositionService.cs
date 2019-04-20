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
using AniCharades.Services.Charades;
using AniCharades.Services.Charades.EntryProcessing;
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
        private IEntryProcessingStrategy entryProcessingStrategy;
        private bool includeKnownAdaptations = false;

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

        public void StartComposing(ICollection<IListEntry> entires, EntrySource source)
        {
            Reset();
            sources.Enqueue(source);
            currentMergedList = entires.ToList();
            entryProcessingStrategy = EntryProcessingFactory.Instance.Get(source);
        }

        public void StartComposing(GetCharadesCriteria criteria)
        {
            Reset();
            usernames = criteria.Usernames;
            sources.EnqueueRange(criteria.Sources);
            if (criteria.IncludeKnownAdaptations && sources.Count != 0)
            {
                var allSources = Enum.GetValues(typeof(EntrySource)).Cast<EntrySource>();
                var isAllDone = criteria.Sources.Count == allSources.Count();
                if (!isAllDone)
                {
                    var otherSources = allSources.Except(criteria.Sources);
                    this.otherSources.EnqueueRange(otherSources);
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
                    includeKnownAdaptations = true;
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
            entryProcessingStrategy = EntryProcessingFactory.Instance.Get(currentSource);
            switch (currentSource)
            {
                case EntrySource.Anime:
                    currentMergedList = (await myAnimeListService.GetMergedAnimeLists(usernames)).ToList();
                    break;
                case EntrySource.Manga:
                    currentMergedList = (await myAnimeListService.GetMergedMangaLists(usernames)).ToList();
                    break;
                default:
                    break;
            }
        }

        private async Task<CharadesEntry> CreateNextCharades(IListEntry entry)
        {
            var existingCharadesWithEntry = entryProcessingStrategy.EntryExistsInCharades(entry, charades);
            if (existingCharadesWithEntry != null)
            {
                return existingCharadesWithEntry;
            }
            var seriesExistsInDb = await entryProcessingStrategy.EntryExistsInRepository(entry, seriesRepository);
            if (seriesExistsInDb)
            {
                var franchiseFromRepo = await entryProcessingStrategy.GetFranchiseFromRepository(entry, seriesRepository);
                return EnsureCreateAndAddToCharades(franchiseFromRepo);
            }
            var franchise = entryProcessingStrategy.CreateFranchise(entry, franchiseService, includeKnownAdaptations);
            var indirectExistingRelation = GetIndirectExistingRelation(charades, franchise);
            if (indirectExistingRelation != null)
            {
                entryProcessingStrategy.AddEntryToCharadesEntry(indirectExistingRelation, entry, franchise);
                return indirectExistingRelation;
            }
            return EnsureCreateAndAddToCharades(franchise);
        }

        private CharadesEntry CreateAndAddCharadesEntry(SeriesEntry franchise)
        {
            var charadesEntry = CreateCharadesEntry(franchise);
            charades.Add(charadesEntry);
            return charadesEntry;
        }

        private CharadesEntry CreateCharadesEntry(SeriesEntry franchise)
        {
            var charadesEntry = new CharadesEntry()
            {
                Series = franchise,
                KnownBy = GetAllUsersForFranchise(franchise, entryProcessingStrategy.GetFranchiseIds(franchise))
            };
            return charadesEntry;
        }

        private void Reset()
        {
            charades = new List<CharadesEntry>();
            sources.Clear();
            otherSources.Clear();
            nextEntryIndex = 0;
            currentMergedList.Clear();
            includeKnownAdaptations = false;
        }

        private ICollection<string> GetAllUsersForFranchise(SeriesEntry franchise, ICollection<long> ids)
        {
            var usernames = new List<string>();
            foreach (var id in ids)
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

        private CharadesEntry EnsureCreateAndAddToCharades(SeriesEntry franchise)
        {
            if (!includeKnownAdaptations)
            {
                return CreateAndAddCharadesEntry(franchise);
            }
            if (entryProcessingStrategy.HasAdaptations(franchise))
            {
                return CreateAndAddCharadesEntry(franchise);
            }
            return null;
        }

        public CharadesEntry GetIndirectExistingRelation(ICollection<CharadesEntry> charades, SeriesEntry franchise)
        {
            var indirectExistingRelation = charades
                .FirstOrDefault(c =>
                {
                    bool hasAnime = c.Series.AnimePositions != null && c.Series.AnimePositions.Any(a => franchise.AnimePositions.Any(f => f.MalId == a.MalId));
                    if (hasAnime)
                        return true;
                    bool hasManga = c.Series.MangaPositions != null && c.Series.MangaPositions.Any(a => franchise.MangaPositions.Any(f => f.MalId == a.MalId));
                    return hasManga;
                });
            return indirectExistingRelation;
        }
    }
}
