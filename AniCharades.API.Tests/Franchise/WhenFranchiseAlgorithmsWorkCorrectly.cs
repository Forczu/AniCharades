﻿using AniCharades.Adapters.Jikan;
using AniCharades.API.Algorithms.Franchise;
using AniCharades.API.Tests.LargeMocks;
using JikanDotNet;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AniCharades.API.Tests.Franchise
{
    public class WhenFranchiseAlgorithmsWorkCorrectly
    {
        private readonly Mock<IJikan> jikanMock;
        private readonly IConfigurationRoot config;
        private readonly FranchiseCreator franchiseCreator;
        private readonly Dictionary<string, long[]> franchises = new Dictionary<string, long[]>();

        public WhenFranchiseAlgorithmsWorkCorrectly()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
            var jikanMockBuilder = new JikanMockBuilder();
            var franchisesOfInterest = config.GetSection("Jikan:Anime:Franchises").GetChildren().Select(c => c.Key).ToArray();
            foreach (var franchiseName in franchisesOfInterest)
            {
                var entries = config.GetSection($"Jikan:Anime:Franchises:{franchiseName}").Get<long[]>();
                franchises[franchiseName] = entries;
                jikanMockBuilder.HasAnimes(entries);
            }
            jikanMock = jikanMockBuilder.Build();
            franchiseCreator = new FranchiseCreator();
        }

        [Theory]
        [InlineData("Nyaruko", "Haiyore! Nyaruko-san", "https://cdn.myanimelist.net/images/anime/6/49081.jpg")]
        [InlineData("LoveLive", "Love Live! School Idol Project", "https://cdn.myanimelist.net/images/anime/11/56849.jpg")]
        [InlineData("KamiNomi", "Kami nomi zo Shiru Sekai", "https://cdn.myanimelist.net/images/anime/2/43361.jpg")]
        [InlineData("CodeGeass", "Code Geass: Hangyaku no Lelouch", "https://cdn.myanimelist.net/images/anime/5/50331.jpg")]
        public void SeriesShouldHaveFirstTvSeriesSelectedAsMain(string franchiseName, string expectedTitle, string expectedImageUrl)
        {
            // given
            var animeAdapters = franchises[franchiseName].Select(n => new JikanAnimeAdapter(jikanMock.Object.GetAnime(n).Result)).ToArray();
            // when
            var series = franchiseCreator.Create(animeAdapters);
            // then
            Assert.Equal(expectedTitle, series.Title);
            Assert.Equal(expectedImageUrl, series.ImageUrl);
        }
    }
}