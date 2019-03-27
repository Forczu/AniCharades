using AniCharades.Adapters.Jikan;
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

        [Fact]
        public void NyarukoSeriesShouldHaveFirstTvSeriesSelectedAsMain()
        {
            // given
            var nyarukoAdapters = franchises["Nyaruko"].Select(n => new JikanAnimeAdapter(jikanMock.Object.GetAnime(n).Result)).ToArray();
            // when
            var nyarukoSeries = franchiseCreator.Create(nyarukoAdapters);
            // then
            Assert.Equal("Haiyore! Nyaruko-san", nyarukoSeries.Title);
            Assert.Equal("https://cdn.myanimelist.net/images/anime/6/49081.jpg", nyarukoSeries.ImageUrl);
        }

        [Fact]
        public void LoveLiveSeriesShouldHaveFirstTvSeriesSelectedAsMain()
        {
            // given
            var loveLiveAdapters = franchises["LoveLive"].Select(n => new JikanAnimeAdapter(jikanMock.Object.GetAnime(n).Result)).ToArray();
            // when
            var loveLiveSeries = franchiseCreator.Create(loveLiveAdapters);
            // then
            Assert.Equal("Love Live! School Idol Project", loveLiveSeries.Title);
            Assert.Equal("https://cdn.myanimelist.net/images/anime/11/56849.jpg", loveLiveSeries.ImageUrl);
        }

        [Fact]
        public void KamiNomiSeriesShouldHaveFirstTvSeriesSelectedAsMain()
        {
            // given
            var kamiNomiEntries = franchises["KamiNomi"].Select(n => new JikanAnimeAdapter(jikanMock.Object.GetAnime(n).Result)).ToArray();
            // when
            var kamiNomiSeries = franchiseCreator.Create(kamiNomiEntries);
            // then
            Assert.Equal("Kami nomi zo Shiru Sekai", kamiNomiSeries.Title);
            Assert.Equal("https://cdn.myanimelist.net/images/anime/2/43361.jpg", kamiNomiSeries.ImageUrl);
        }
    }
}