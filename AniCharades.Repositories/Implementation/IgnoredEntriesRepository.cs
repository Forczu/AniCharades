using AniCharades.Contracts.Enums;
using AniCharades.Data.Context;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AniCharades.Repositories.Implementation
{
    public class IgnoredEntriesRepository : IIgnoredEntriesRepository
    {
        private readonly DataContext context;

        public IgnoredEntriesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<IgnoredEntry> Get(long id, EntrySource source)
        {
            return await context.Ignored.FirstOrDefaultAsync(i => i.Id == id && i.Source == source);
        }

        public async Task<bool> IsIgnored(long id, EntrySource source)
        {
            return await context.Ignored.AnyAsync(i => i.Id == id && i.Source == source);
        }
    }
}
