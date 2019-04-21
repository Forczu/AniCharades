using AniCharades.Contracts.Enums;
using AniCharades.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AniCharades.Repositories.Interfaces
{
    public interface IIgnoredEntriesRepository
    {
        Task<IgnoredEntry> Get(long id, EntrySource source);

        Task<bool> IsIgnored(long id, EntrySource source);
    }
}
