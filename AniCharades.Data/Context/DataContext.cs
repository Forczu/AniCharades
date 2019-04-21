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
        public DbSet<IgnoredEntry> Ignored { get; set; }
        public DbSet<RelationCriteria> RelationCriterias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SeriesEntry>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.ImageUrl).IsRequired();

                entity.HasOne(s => s.Translation)
                    .WithOne(t => t.Series)
                    .HasForeignKey<Translation>(t => t.SeriesId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(s => s.AnimePositions)
                    .WithOne(a => a.Series)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(s => s.MangaPositions)
                    .WithOne(m => m.Series)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Id);
                entity.HasIndex(e => e.Title);
            });

            modelBuilder.Entity<AnimeEntry>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();
                entity.HasIndex(e => e.MalId);
                entity.HasIndex(e => e.Title);
            });

            modelBuilder.Entity<MangaEntry>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();
                entity.HasIndex(e => e.MalId);
                entity.HasIndex(e => e.Title);
            });

            modelBuilder.Entity<IgnoredEntry>(entity =>
            {
                entity.HasKey(e => new { e.Source, e.Id });
                entity.HasIndex(e => e.Source);
                entity.HasIndex(e => e.Id);
            });

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
