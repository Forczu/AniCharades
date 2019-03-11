﻿using AniCharades.API.Models;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
               .Entity<SeriesEntry>()
               .Property(e => e.AnimePositions)
               .HasConversion(DataConverters.IntArrayToStringConverter);
            modelBuilder
               .Entity<SeriesEntry>()
               .Property(e => e.MangaPositions)
               .HasConversion(DataConverters.IntArrayToStringConverter);
            modelBuilder
               .Entity<SeriesEntry>()
               .Property(e => e.Translations)
               .HasConversion(DataConverters.StringArrayToStringConverter);
        }
    }
}