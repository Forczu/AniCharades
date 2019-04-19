using AniCharades.Adapters.Interfaces;
using AniCharades.Contracts.Charades;
using AniCharades.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Services.Interfaces
{
    public interface ICharadesCompositionService
    {
        Task<ICollection<CharadesEntry>> GetCharades(GetCharadesCriteria criteria);

        Task StartComposing(ICollection<IListEntry> entires);

        Task StartComposing(ICollection<string> usernames);

        Task<ICollection<CharadesEntry>> GetFinishedCharades();

        int GetMergedPositionsCount();

        Task<CharadesEntry> MakeNextCharadesEntry();
    }
}
