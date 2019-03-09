using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AniCharades.API.Data
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly DataContext context;

        public SeriesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<SeriesEntry>> Get()
        {
            return await context.Series.ToListAsync();
        }

        public async Task<SeriesEntry> Get(int id)
        {
            return await context.Series.FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
