using AniCharades.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<SeriesEntry> Series { get; set; }
        public DbSet<AnimeEntry> Animes { get; set; }
        public DbSet<MangaEntry> Mangas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
               .Entity<SeriesEntry>()
               .Property(e => e.Translations)
               .HasConversion(DataConverters.StringArrayToStringConverter);
        }
    }
}
