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
    public class WhenSeriesAssemblerWorksCorrectly
    {
        private readonly IFranchiseService franchiseService;
        private IConfigurationRoot config;

        public WhenSeriesAssemblerWorksCorrectly()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
            var jikanMock = new JikanMockBuilder()
                .HasAnimes(config.GetSection("Jikan:Anime:Franchises:Nyaruko").Get<long[]>())
                .HasAnimes(config.GetSection("Jikan:Anime:Franchises:KamiNomi").Get<long[]>())
                .Build();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(JikanAnimeProvider))).Returns(new JikanAnimeProvider(jikanMock.Object));
            serviceProvider.Setup(s => s.GetService(typeof(JikanMangaProvider))).Returns(new JikanMangaProvider(jikanMock.Object));
            franchiseService = new FranchiseService(serviceProvider.Object, new RelationService());
        }

        [Fact]
        public void NyarukoFranchiseShouldHaveAllTitles()
        {
            // given
            long nyarukoId = 11785;
            // when
            var nyarukoSeries = franchiseService.CreateFromAnime(nyarukoId);
            // then
            Assert.Equal(8, nyarukoSeries.AnimePositions.Count);
            Assert.True(nyarukoSeries.AnimePositions.GroupBy(x => x).All(g => g.Count() == 1));
        }

        [Fact]
        public void KamiNomiFranchiseShouldContainMagicalStarKanon()
        {
            // given
            long kamiNomiId = 8525;
            // when
            var kamiNomiFranchise = franchiseService.CreateFromAnime(kamiNomiId);
            // then
            Assert.Equal(9, kamiNomiFranchise.AnimePositions.Count);
            Assert.Contains(kamiNomiFranchise.AnimePositions, a => a.MalId == 17725);
            Assert.True(kamiNomiFranchise.AnimePositions.GroupBy(x => x).All(g => g.Count() == 1));
        }
    }
}
