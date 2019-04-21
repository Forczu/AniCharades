using AniCharades.Algorithms.MyAnimeList;
using AniCharades.API.Tests.LargeMocks;
using AniCharades.Contracts.Enums;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Franchise;
using AniCharades.Services.Franchise.Providers;
using AniCharades.Services.Implementation;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;

namespace AniCharades.API.Tests.Charades
{
    public class CharadesCompositionServiceFixture : IDisposable
    {
        public ICharadesCompositionService Object { get; private set; }
        public IConfigurationRoot Config { get; }

        public CharadesCompositionServiceFixture()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();

            Mock<JikanDotNet.IJikan> jikanMock = GetJikan();
            MyAnimeListService myAnimeListService = GetMyAnimeListService(jikanMock);
            Mock<ISeriesRepository> seriesRepository = GetSeriesRepository();
            FranchiseService franchiseService = GetFranchiseService(jikanMock);
            Object = new CharadesCompositionService(myAnimeListService, seriesRepository.Object, franchiseService);
        }

        public void Dispose()
        {
            Object = null;
        }

        private Mock<JikanDotNet.IJikan> GetJikan()
        {
            return new JikanMockBuilder()
                .HasUserAnimeList("Ervelan")
                .HasUserAnimeList("SonMati")
                .HasUserMangaList("Ervelan")
                .HasUserMangaList("Progeusz")
                .HasUserMangaList("SonMati")
                .HasAllAnimes()
                .HasAllMangas()
                .Build();
        }

        private MyAnimeListService GetMyAnimeListService(Mock<JikanDotNet.IJikan> jikanMock)
        {
            var listExtractorMock = new ListExtractor(jikanMock.Object);
            var myAnimeListService = new MyAnimeListService(listExtractorMock);
            return myAnimeListService;
        }

        private Mock<ISeriesRepository> GetSeriesRepository()
        {
            var seriesRepository = new Mock<ISeriesRepository>();
            PrepareDateALiveFranchise(seriesRepository);
            seriesRepository.SetReturnsDefault(false);
            return seriesRepository;
        }

        private void PrepareDateALiveFranchise(Mock<ISeriesRepository> seriesRepository)
        {
            var dateALiveFranchiseWithoutS3 = new SeriesEntry()
            {
                Title = "Date A Live",
                AnimePositions = new List<AnimeEntry>()
                {
                    new AnimeEntry() { MalId = 15583, Title = "Date A Live" },
                    new AnimeEntry() { MalId = 17641, Title = "Date A Live: Date to Date" },
                    new AnimeEntry() { MalId = 19163, Title = "Date A Live II" },
                    new AnimeEntry() { MalId = 22961, Title = "Date A Live: Kurumi Star Festival" },
                    new AnimeEntry() { MalId = 24655, Title = "Date A Live: Mayuri Judgment" }
                }
            };
            foreach (var dalEntryId in new[] { 15583, 17641, 19163, 22961, 24655 })
            {
                seriesRepository.Setup(r => r.GetByAnimeId(dalEntryId)).ReturnsAsync(dateALiveFranchiseWithoutS3);
                seriesRepository.Setup(r => r.SeriesExistsByAnimeId(dalEntryId)).ReturnsAsync(true);
            }
            seriesRepository.Setup(r => r.SeriesExistsByAnimeId(36633)).ReturnsAsync(false);
        }

        private FranchiseService GetFranchiseService(Mock<JikanDotNet.IJikan> jikanMock)
        {
            var ignoredRepo = new Mock<IIgnoredEntriesRepository>();
            ignoredRepo.SetReturnsDefault(false);
            var ignoredAnimeIds = Config.GetSection($"Ignored:Anime").Get<long[]>();
            foreach (var id in ignoredAnimeIds)
            {
                ignoredRepo.Setup(r => r.IsIgnored(id, EntrySource.Anime)).ReturnsAsync(true);
            }
            var providerFactory = new Mock<IEntryProviderFactory>();
            providerFactory.Setup(s => s.Get(EntrySource.Anime)).Returns(new JikanAnimeProvider(jikanMock.Object, ignoredRepo.Object));
            providerFactory.Setup(s => s.Get(EntrySource.Manga)).Returns(new JikanMangaProvider(jikanMock.Object, ignoredRepo.Object));
            var franchiseService = new FranchiseService(providerFactory.Object, new FranchiseAssembler(new RelationService()));
            return franchiseService;
        }
    }
}
