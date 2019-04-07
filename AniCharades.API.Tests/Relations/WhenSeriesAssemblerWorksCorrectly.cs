﻿using AniCharades.API.Tests.LargeMocks;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Franchise;
using AniCharades.Services.Implementation;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AniCharades.API.Tests.Relations
{
    public class WhenSeriesAssemblerWorksCorrectly : BaseTest
    {
        private readonly IFranchiseService franchiseService;

        private static readonly Dictionary<string, long> malDictionary = new Dictionary<string, long>()
        {
            { "KamiNomiFirstTv", 8525 }, { "MagicalStarKanon", 17725 },
            { "NyarukoFirstTv", 11785 },
            { "KaijiFirstTv", 3002 }, { "TonegawaTv", 37338 },
            { "SakiFirstTv", 5671 }, { "SakiAchigaTv", 10884 },
            { "EfMemoriesTv", 2924 }, { "EfMelodiesTv", 4789 },
            { "ClannadTv", 2167 },
            { "SataniaDropoutSpecials", 34855 },
            { "GintamaSecondTv", 9969 }, { "GintamaThirdTv", 15417 }, { "SketDanceTv", 9863 }, { "GintamaMameshiba", 19261 },
            { "MahoukaTv", 20785 }, { "MahoukaMameshiba", 35631 },
            { "LoveLiveSecondTv", 19111 },
            { "ZombieDesuKaFirstTv", 8841 },{ "ZombieDesuKaSecondTv", 10790 },
            { "ChuunibyouRenLite", 21797 },
            { "DragonBallGT", 225 }, { "DrSlumpTv", 2222 }
        };

        public WhenSeriesAssemblerWorksCorrectly()
        {
            var jikanMockBuilder = new JikanMockBuilder();
            var franchises = Config.GetSection("Jikan:Anime:Franchises").GetChildren().Select(c => c.Key).ToArray();
            foreach (var franchise in franchises)
            {
                jikanMockBuilder.HasAnimes(Config.GetSection($"Jikan:Anime:Franchises:{franchise}").Get<long[]>());
            }
            var jikanMock = jikanMockBuilder.Build();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(JikanAnimeProvider))).Returns(new JikanAnimeProvider(jikanMock.Object));
            serviceProvider.Setup(s => s.GetService(typeof(JikanMangaProvider))).Returns(new JikanMangaProvider(jikanMock.Object));
            franchiseService = new FranchiseService(serviceProvider.Object, new FranchiseAssembler(new RelationService()));
        }

        [Theory]
        [InlineData("NyarukoFirstTv", 8)]
        [InlineData("KamiNomiFirstTv",  9)]
        [InlineData("KaijiFirstTv",  2)]
        [InlineData("SakiFirstTv",  7)]
        [InlineData("EfMemoriesTv",  5)]
        [InlineData("ClannadTv",  5)]
        [InlineData("SataniaDropoutSpecials",  2)]
        [InlineData("MahoukaTv", 4)]
        [InlineData("GintamaThirdTv", 19)]
        [InlineData("ChuunibyouRenLite", 13)]
        [InlineData("LoveLiveSecondTv", 25)]
        [InlineData("DragonBallGT", 41)]
        public void FranchiseShouldHaveExpectedCount(string firstEntryName, int expectedCount)
        {
            // given
            long firstId = malDictionary[firstEntryName];
            // when
            var franchise = franchiseService.CreateFromAnime(firstId);
            // then
            Assert.Equal(expectedCount, franchise.AnimePositions.Count);
            Assert.True(franchise.AnimePositions.GroupBy(x => x).All(g => g.Count() == 1));
        }

        [Theory]
        [InlineData("EfMemoriesTv", "EfMelodiesTv")]
        [InlineData("ZombieDesuKaSecondTv", "ZombieDesuKaFirstTv")]
        public void FranchisesMadeFromSharedEntriesShouldHaveEqualLength(string firstEntryName, string secondEntryName)
        {
            // given
            long firstId = malDictionary[firstEntryName];
            long secondId = malDictionary[secondEntryName];
            // when
            var firstFranchise = franchiseService.CreateFromAnime(firstId);
            var secondFranchise = franchiseService.CreateFromAnime(secondId);
            // then
            Assert.Equal(firstFranchise.AnimePositions.Count, secondFranchise.AnimePositions.Count);
        }

        [Theory]
        [InlineData("KamiNomiFirstTv", "MagicalStarKanon")]
        [InlineData("SakiFirstTv", "SakiAchigaTv")]
        public void FranchiseShouldContainCertainEntry(string firstEntryName, string expectedEntryName)
        {
            // given
            long firstId = malDictionary[firstEntryName];
            long expectedEntryId = malDictionary[expectedEntryName];
            // when
            var franchise = franchiseService.CreateFromAnime(firstId);
            // then
            Assert.Contains(franchise.AnimePositions, a => a.MalId == expectedEntryId);
        }

        [Theory]
        [InlineData("KaijiFirstTv", "TonegawaTv")]
        [InlineData("TonegawaTv", "KaijiFirstTv")]
        [InlineData("GintamaThirdTv", "SketDanceTv")]
        [InlineData("GintamaSecondTv", "GintamaMameshiba")]
        [InlineData("MahoukaTv", "MahoukaMameshiba")]
        [InlineData("DragonBallGT", "DrSlumpTv")]
        public void FranchiseShouldNotContainCertainEntry(string firstEntryName, string expectedNonContainedEntryName)
        {
            // given
            long firstId = malDictionary[firstEntryName];
            long expectedNonContainedEntryId = malDictionary[expectedNonContainedEntryName];
            // when
            var franchise = franchiseService.CreateFromAnime(firstId);
            // then
            Assert.DoesNotContain(franchise.AnimePositions, a => a.MalId == expectedNonContainedEntryId);
        }
    }
}
