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
            var existingCharadesWithEntry = charades.FirstOrDefault(c => c.Series.AnimePositions.Any(a => a.MalId == entry.Id));
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

        public SeriesEntry CreateFranchise(IListEntry entry, IFranchiseService franchiseService)
        {
            var franchise = franchiseService.CreateFromAnime(entry.Id);
            return franchise;
        }

        public CharadesEntry GetIndirectExistingRelation(ICollection<CharadesEntry> charades, SeriesEntry franchise)
        {
            var indirectExistingRelation = charades
                .FirstOrDefault(c => c.Series.AnimePositions
                    .Any(a => franchise.AnimePositions
                        .Any(f => f.MalId == a.MalId)));
            return indirectExistingRelation;
        }

        public void AddEntryToCharadesEntry(CharadesEntry charadesEntry, IListEntry entry)
        {
            charadesEntry.Series.AnimePositions.Add(new AnimeEntry() { MalId = entry.Id, Title = entry.Title, Series = charadesEntry.Series });
            var newUsers = entry.Users.Where(u => !charadesEntry.KnownBy.Contains(u)).ToArray();
            newUsers.ForEach(u => charadesEntry.KnownBy.Add(u));
        }

        public ICollection<long> GetFranchiseIds(SeriesEntry franchise)
        {
            return franchise.AnimePositions.Select(a => a.MalId).ToArray();
        }
    }
}
