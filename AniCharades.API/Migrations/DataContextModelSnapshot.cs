﻿// <auto-generated />
using AniCharades.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AniCharades.API.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085");

            modelBuilder.Entity("AniCharades.API.Models.SeriesEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AnimePositions");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("MangaPositions");

                    b.Property<string>("Title");

                    b.Property<string>("Translations");

                    b.HasKey("Id");

                    b.ToTable("Series");
                });
#pragma warning restore 612, 618
        }
    }
}
