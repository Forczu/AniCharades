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
        private static readonly string[] Franchises = { "Kaiji", "KamiNomi", "Nyaruko", "Saki" };

        private readonly IFranchiseService franchiseService;

        public WhenSeriesAssemblerWorksCorrectly()
        {
            var jikanMockBuilder = new JikanMockBuilder();
            foreach (var franchise in Franchises)
            {
                jikanMockBuilder.HasAnimes(Config.GetSection($"Jikan:Anime:Franchises:{franchise}").Get<long[]>());
            }
            var jikanMock = jikanMockBuilder.Build();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(JikanAnimeProvider))).Returns(new JikanAnimeProvider(jikanMock.Object));
            serviceProvider.Setup(s => s.GetService(typeof(JikanMangaProvider))).Returns(new JikanMangaProvider(jikanMock.Object));
            franchiseService = new FranchiseService(serviceProvider.Object, new RelationService());
        }

        [Theory]
        [InlineData(11785, 8)] // Nyaruko
        [InlineData(8525,  9)] // KamiNomi
        [InlineData(3002,  2)] // Kaiji
        [InlineData(5671,  7)] // Saki
        public void FranchiseShouldHaveExpectedCount(int malEntryId, int expectedCount)
        {
            // given
            long firstId = malEntryId;
            // when
            var franchise = franchiseService.CreateFromAnime(firstId);
            // then
            Assert.Equal(expectedCount, franchise.AnimePositions.Count);
            Assert.True(franchise.AnimePositions.GroupBy(x => x).All(g => g.Count() == 1));
        }

        [Theory]
        [InlineData(8525, 17725)] // KamiNomi, Magical Star Kanon
        [InlineData(5671, 10884)] // Saki, Achiga
        public void FranchiseShouldContainCertainEntry(int malEntryId, int expectedEntryId)
        {
            // given
            long firstId = malEntryId;
            // when
            var franchise = franchiseService.CreateFromAnime(firstId);
            // then
            Assert.Contains(franchise.AnimePositions, a => a.MalId == expectedEntryId);
        }

        [Theory]
        [InlineData(3002, 37338)] // Kaiji, Tonegawa
        public void FranchiseShouldNotContainCertainEntry(int malEntryId, int expectedNonContainedEntryId)
        {
            // given
            long firstId = malEntryId;
            // when
            var franchise = franchiseService.CreateFromAnime(firstId);
            // then
            Assert.DoesNotContain(franchise.AnimePositions, a => a.MalId == expectedNonContainedEntryId);
        }
    }
}
