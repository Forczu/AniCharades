using AniCharades.API.Tests.LargeMocks;
using AniCharades.Common.Extensions;
using AniCharades.Contracts.Enums;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Franchise;
using AniCharades.Services.Franchise.Providers;
using AniCharades.Services.Implementation;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AniCharades.API.Tests.Relations
{
    public class WhenSeriesAssemblerWorksCorrectly : BaseTest
    {
        private readonly IFranchiseService franchiseService;
        private readonly Dictionary<string, long> malDictionary = new Dictionary<string, long>();

        public WhenSeriesAssemblerWorksCorrectly()
        {
            var jikanMockBuilder = new JikanMockBuilder();
            var franchises = Config.GetSection("Jikan:Anime:Franchises").GetChildren().Select(c => c.Key).ToArray();
            foreach (var franchise in franchises)
            {
                jikanMockBuilder.HasAnimes(Config.GetSection($"Jikan:Anime:Franchises:{franchise}").Get<long[]>());
            }
            franchises = Config.GetSection("Jikan:Manga:Franchises").GetChildren().Select(c => c.Key).ToArray();
            foreach (var franchise in franchises)
            {
                jikanMockBuilder.HasMangas(Config.GetSection($"Jikan:Manga:Franchises:{franchise}").Get<long[]>());
            }
            var jikanMock = jikanMockBuilder.Build();
            var ignoredRepo = new Mock<IIgnoredEntriesRepository>();
            ignoredRepo.SetReturnsDefault(false);
            var ignoredAnimeIds = Config.GetSection($"Ignored:Anime").Get<long[]>();
            foreach (var id in ignoredAnimeIds)
            {
                ignoredRepo.Setup(r => r.IsIgnored(id, EntrySource.Anime)).ReturnsAsync(true);
            }
            var serviceProvider = new Mock<IEntryProviderFactory>();
            serviceProvider.Setup(s => s.Get(EntrySource.Anime)).Returns(new JikanAnimeProvider(jikanMock.Object, ignoredRepo.Object));
            serviceProvider.Setup(s => s.Get(EntrySource.Manga)).Returns(new JikanMangaProvider(jikanMock.Object, ignoredRepo.Object));
            franchiseService = new FranchiseService(serviceProvider.Object, new FranchiseAssembler(new RelationService()));

            var values = Config.GetSection("Jikan:Entries").GetChildren().Select(c => new { c.Key, Value = long.Parse(c.Value) });
            values.ForEach(v => malDictionary.Add(v.Key, v.Value));
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
        [InlineData("FairyTailTv", 9)]
        [InlineData("FateFirstTv", 11)]
        [InlineData("PrismaIllyaFirstTv", 13)]
        [InlineData("GintamaThirdTv", 19)]
        [InlineData("ChuunibyouRenLite", 13)]
        [InlineData("LoveLiveSecondTv", 25)]
        [InlineData("DragonBallGT", 41)]
        [InlineData("LupinFirstTv", 52)]
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
        [InlineData("FateFirstTv", "FateUbwMovie")]
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
        [InlineData("PrismaIllyaFirstTv", "PrismaIllyaMovieSpecial")]
        [InlineData("FateGrandOrderFirstOva", "FateGrandOrderCMs")]
        [InlineData("FateGrandOrderFirstOva", "FateGrandOrderMangaDeWakaru")]
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
        [InlineData("GintamaThirdTv", "SketDanceTv")]
        [InlineData("GintamaSecondTv", "GintamaMameshiba")]
        [InlineData("MahoukaTv", "MahoukaMameshiba")]
        [InlineData("DragonBallGT", "DrSlumpTv")]
        [InlineData("KimiGaNozomuEienTv", "MobileSuitGundamFirstTv")]
        [InlineData("LupinFirstTv", "LupinVsConanMovie")]
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

        [Theory]
        [InlineData("KaijiFirstTv", "TonegawaTv")]
        [InlineData("FateUbwMovie", "FateZeroFirstTv")]
        [InlineData("FateGrandOrderFirstOva", "FateExtraTv")]
        [InlineData("FateFirstTv", "PrismaIllyaFirstTv")]
        [InlineData("MajiKoiTv", "KimiAruTv")]
        public void AnimeFranchisesShouldBeSeparate(string firstEntryName, string secondEntryName)
        {
            // given
            long firstId = malDictionary[firstEntryName];
            long secondId = malDictionary[secondEntryName];
            // when
            var firstFranchise = franchiseService.CreateFromAnime(firstId);
            var secondFranchise = franchiseService.CreateFromAnime(secondId);
            // then
            Assert.DoesNotContain(firstFranchise.AnimePositions, a => a.MalId == secondId);
            Assert.DoesNotContain(secondFranchise.AnimePositions, a => a.MalId == firstId);
        }

        [Theory]
        [InlineData("BlameManga", "SidoniaNoKishiManga")]
        public void MangaFranchisesShouldBeSeparate(string firstEntryName, string secondEntryName)
        {
            // given
            long firstId = malDictionary[firstEntryName];
            long secondId = malDictionary[secondEntryName];
            // when
            var firstFranchise = franchiseService.CreateFromManga(firstId);
            var secondFranchise = franchiseService.CreateFromManga(secondId);
            // then
            Assert.DoesNotContain(firstFranchise.MangaPositions, a => a.MalId == secondId);
            Assert.DoesNotContain(secondFranchise.MangaPositions, a => a.MalId == firstId);
        }

        [Theory]
        [InlineData("FairyTailManga", 11)]
        public void MangaFranchiseShouldHaveExpectedCount(string firstEntryName, int expectedCount)
        {
            // given
            long firstId = malDictionary[firstEntryName];
            // when
            var franchise = franchiseService.CreateFromManga(firstId);
            // then
            Assert.Equal(expectedCount, franchise.MangaPositions.Count);
            Assert.True(franchise.MangaPositions.GroupBy(x => x).All(g => g.Count() == 1));
        }

        [Fact]
        public void IgnoredEntriewsShouldNotAppearInFranchse()
        {
            // given
            var hanasakuId = malDictionary["HanasakuIrohaTv"];
            var utopiaId = malDictionary["UtopiaMusic"];
            // when
            var franchise = franchiseService.CreateFromAnime(hanasakuId);
            // then
            Assert.Contains(franchise.AnimePositions, a => a.MalId == hanasakuId);
            Assert.DoesNotContain(franchise.AnimePositions, a => a.MalId == utopiaId);
        }

        [Fact]
        public void IgnoredEntryShouldNotCreateFranchise()
        {
            // given
            var hanasakuId = malDictionary["SekiranunGraffiti"];
            // when
            var franchise = franchiseService.CreateFromAnime(hanasakuId);
            // then
            Assert.Null(franchise);
        }

        [Fact]
        public void CreatingFromAnimeShouldNotContainWrongMangas()
        {
            // given
            var tsubasaId = malDictionary["TsubasaChronicleFirstTv"];
            var xxxHolicId = malDictionary["xxxHolicMangaId"];
            // when
            var franchise = franchiseService.CreateFromAnime(tsubasaId, AdaptationIncluding.All);
            // then
            Assert.DoesNotContain(franchise.MangaPositions, m => m.MalId == xxxHolicId);
        }
    }
}
