using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Contracts.Enums;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Interfaces;

namespace AniCharades.Services.Charades.EntryProcessing
{
    public class MangaProcessingStrategy : IEntryProcessingStrategy
    {
        public CharadesEntry EntryExistsInCharades(IListEntry entry, ICollection<CharadesEntry> charades)
        {
            var existingCharadesWithEntry = charades
                .FirstOrDefault(c => c.Series.MangaPositions != null && c.Series.MangaPositions.Any(a => a.MalId == entry.Id));
            return existingCharadesWithEntry;
        }

        public async Task<bool> EntryExistsInRepository(IListEntry entry, ISeriesRepository seriesRepository)
        {
            var seriesExistsInDb = await seriesRepository.SeriesExistsByMangaId(entry.Id);
            return seriesExistsInDb;
        }

        public async Task<SeriesEntry> GetFranchiseFromRepository(IListEntry entry, ISeriesRepository seriesRepository)
        {
            var franchise = await seriesRepository.GetByMangaId(entry.Id);
            return franchise;
        }

        public SeriesEntry CreateFranchise(IListEntry entry, IFranchiseService franchiseService, AdaptationIncluding withAdaptations)
        {
            var franchise = franchiseService.CreateFromManga(entry.Id, withAdaptations);
            return franchise;
        }

        public void AddEntryToCharadesEntry(CharadesEntry charadesEntry, IListEntry entry, SeriesEntry franchise)
        {
            charadesEntry.Series.MangaPositions.Add(new MangaEntry() { MalId = entry.Id, Title = entry.Title, Series = charadesEntry.Series });
            var newUsers = entry.Users.Where(u => !charadesEntry.KnownBy.Contains(u)).ToArray();
            newUsers.ForEach(u => charadesEntry.KnownBy.Add(u));

            franchise.AnimePositions
                .Where(m => !charadesEntry.Series.AnimePositions.Any(a => a.MalId == m.MalId))
                .ForEach(m => charadesEntry.Series.AnimePositions.Add(m));
        }

        public ICollection<long> GetFranchiseIds(SeriesEntry franchise)
        {
            return franchise.MangaPositions.Select(a => a.MalId).ToArray();
        }

        public bool HasAdaptations(SeriesEntry franchise)
        {
            return franchise.AnimePositions != null && franchise.AnimePositions.Count != 0;
        }
    }
}
