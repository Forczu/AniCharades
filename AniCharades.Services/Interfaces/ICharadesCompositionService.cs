using AniCharades.Adapters.Interfaces;
using AniCharades.Contracts.Charades;
using AniCharades.Contracts.Enums;
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

        void StartComposing(ICollection<IListEntry> entires, EntrySource source);

        void StartComposing(GetCharadesCriteria criteria);

        ICollection<CharadesEntry> GetFinishedCharades();

        bool IsFinished();

        Task<CharadesEntry> MakeNextCharadesEntry();
    }
}
