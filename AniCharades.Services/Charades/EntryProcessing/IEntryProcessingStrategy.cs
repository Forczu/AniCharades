using AniCharades.Adapters.Interfaces;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AniCharades.Services.Charades.EntryProcessing
{
    public interface IEntryProcessingStrategy
    {
        CharadesEntry EntryExistsInCharades(IListEntry entry, ICollection<CharadesEntry> charades);

        Task<bool> EntryExistsInRepository(IListEntry entry, ISeriesRepository seriesRepository);

        Task<SeriesEntry> GetFranchiseFromRepository(IListEntry entry, ISeriesRepository seriesRepository);

        SeriesEntry CreateFranchise(IListEntry entry, IFranchiseService franchiseService, bool includeAdaptations);

        void AddEntryToCharadesEntry(CharadesEntry charadesEntry, IListEntry entry, SeriesEntry franchise);

        ICollection<long> GetFranchiseIds(SeriesEntry franchise);

        bool HasAdaptations(SeriesEntry franchise);
    }
}
