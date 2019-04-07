using AniCharades.API.Tests.LargeMocks;
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
            { "KamiNomiFirstTv", 8525 },
            { "NyarukoFirstTv", 11785 },
            { "KaijiFirstTv", 3002 },
            { "SakiFirstTv", 5671 },
            { "EfMemoriesTv", 2924 }, { "EfMelodiesTv", 4789 },
            { "ClannadTv", 2167 },
            { "SataniaDropoutSpecials", 34855 },
            { "GintamaThirdTv", 15417 },
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
        [InlineData("EfMelodiesTv",  5)]
        //[InlineData("ClannadTv",  5)]
        [InlineData("SataniaDropoutSpecials",  2)]
        [InlineData("GintamaThirdTv",  19)]
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
        [InlineData("KamiNomiFirstTv", 17725)] // Magical Star Kanon
        [InlineData("SakiFirstTv", 10884)] // Achiga
        public void FranchiseShouldContainCertainEntry(string firstEntryName, int expectedEntryId)
        {
            // given
            long firstId = malDictionary[firstEntryName];
            // when
            var franchise = franchiseService.CreateFromAnime(firstId);
            // then
            Assert.Contains(franchise.AnimePositions, a => a.MalId == expectedEntryId);
        }

        [Theory]
        [InlineData("KaijiFirstTv", 37338)] // Tonegawa
        [InlineData("GintamaThirdTv", 9863)] // SketDance
        public void FranchiseShouldNotContainCertainEntry(string firstEntryName, int expectedNonContainedEntryId)
        {
            // given
            long firstId = malDictionary[firstEntryName];
            // when
            var franchise = franchiseService.CreateFromAnime(firstId);
            // then
            Assert.DoesNotContain(franchise.AnimePositions, a => a.MalId == expectedNonContainedEntryId);
        }
    }
}
