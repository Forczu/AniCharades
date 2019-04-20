using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Services.Charades.EntryProcessing
{
    public class AnimeProcessingStrategy : IEntryProcessingStrategy
    {
        public CharadesEntry EntryExistsInCharades(IListEntry entry, ICollection<CharadesEntry> charades)
        {
            var existingCharadesWithEntry = charades
                .FirstOrDefault(c => c.Series.AnimePositions != null && c.Series.AnimePositions.Any(a => a.MalId == entry.Id));
            return existingCharadesWithEntry;
        }

        public async Task<bool> EntryExistsInRepository(IListEntry entry, ISeriesRepository seriesRepository)
        {
            var seriesExistsInDb = await seriesRepository.SeriesExistsByAnimeId(entry.Id);
            return seriesExistsInDb;
        }

        public async Task<SeriesEntry> GetFranchiseFromRepository(IListEntry entry, ISeriesRepository seriesRepository)
        {
            var franchise = await seriesRepository.GetByAnimeId(entry.Id);
            return franchise;
        }

        public SeriesEntry CreateFranchise(IListEntry entry, IFranchiseService franchiseService, bool includeAdaptations)
        {
            var franchise = franchiseService.CreateFromAnime(entry.Id, includeAdaptations);
            return franchise;
        }

        public void AddEntryToCharadesEntry(CharadesEntry charadesEntry, IListEntry entry, SeriesEntry franchise)
        {
            charadesEntry.Series.AnimePositions.Add(new AnimeEntry() { MalId = entry.Id, Title = entry.Title, Series = charadesEntry.Series });
            var newUsers = entry.Users.Where(u => !charadesEntry.KnownBy.Contains(u)).ToArray();
            newUsers.ForEach(u => charadesEntry.KnownBy.Add(u));

            franchise.MangaPositions
                .Where(m => !charadesEntry.Series.MangaPositions.Any(a => a.MalId == m.MalId))
                .ForEach(m => charadesEntry.Series.MangaPositions.Add(m));
        }

        public ICollection<long> GetFranchiseIds(SeriesEntry franchise)
        {
            return franchise.AnimePositions.Select(a => a.MalId).ToArray();
        }

        public bool HasAdaptations(SeriesEntry franchise)
        {
            return franchise.MangaPositions != null && franchise.MangaPositions.Count != 0;
        }
    }
}
