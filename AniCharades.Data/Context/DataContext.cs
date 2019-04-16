using AniCharades.Data.Converters;
using AniCharades.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Data.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<SeriesEntry> Series { get; set; }
        public DbSet<AnimeEntry> Animes { get; set; }
        public DbSet<MangaEntry> Mangas { get; set; }
        public DbSet<RelationCriteria> RelationCriterias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
               .Entity<RelationCriteria>()
               .Property(e => e.Strategies)
               .HasConversion(DataConverters.StringArrayToStringConverter);
            modelBuilder
               .Entity<RelationCriteria>()
               .Property(e => e.Types)
               .HasConversion(DataConverters.StringArrayToStringConverter);
            modelBuilder
               .Entity<RelationCriteria>()
               .Property(e => e.Keywords)
               .HasConversion(DataConverters.StringArrayToStringConverter);
            modelBuilder
               .Entity<RelationCriteria>()
               .Property(e => e.Relations)
               .HasConversion(DataConverters.RelationTypeArrayToStringConverter);
        }
    }
}
