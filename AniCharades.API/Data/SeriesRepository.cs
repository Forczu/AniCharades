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

        public async Task Add(SeriesEntry series)
        {
            await context.Series.AddAsync(series);
            context.SaveChanges();
        }

        public async Task<IEnumerable<SeriesEntry>> Get()
        {
            return await context.Series.ToListAsync();
        }

        public async Task<SeriesEntry> Get(int id)
        {
            return await context.Series.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SeriesEntry> GetByAnimeId(long id)
        {
            return await context.Series.FirstOrDefaultAsync(s => s.AnimePositions.Any(a => a.MalId == id));
        }

        public async Task<SeriesEntry> GetByMangaId(long id)
        {
            return await context.Series.FirstOrDefaultAsync(s => s.MangaPositions.Any(m => m.MalId == id));
        }

        public async Task<bool> SeriesExists(int id)
        {
            return await context.Series.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> SeriesExistsByAnimeId(long id)
        {
            return await context.Series.AnyAsync(s => s.AnimePositions.Any(a => a.MalId == id));
        }

        public async Task<bool> SeriesExistsByMangaId(long id)
        {
            return await context.Series.AnyAsync(s => s.MangaPositions.Any(m => m.MalId == id));
        }

        public async Task Update(SeriesEntry series)
        {
            var dbEntry = await Get(series.Id);
            dbEntry.Title = series.Title;
            dbEntry.AnimePositions = series.AnimePositions;
            dbEntry.MangaPositions = series.MangaPositions;
            dbEntry.Translations = series.Translations;
            dbEntry.ImageUrl = series.ImageUrl;
            context.Update(dbEntry);
            context.SaveChanges();
        }
    }
}
