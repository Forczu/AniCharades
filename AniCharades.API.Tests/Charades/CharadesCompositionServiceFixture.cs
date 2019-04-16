using AniCharades.Algorithms.MyAnimeList;
using AniCharades.API.Tests.LargeMocks;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Franchise;
using AniCharades.Services.Implementation;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using Moq;
using System;
using System.Collections.Generic;

namespace AniCharades.API.Tests.Charades
{
    public class CharadesCompositionServiceFixture : IDisposable
    {
        public ICharadesCompositionService Object { get; private set; }

        public CharadesCompositionServiceFixture()
        {
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

        private static Mock<JikanDotNet.IJikan> GetJikan()
        {
            return new JikanMockBuilder()
                .HasUserAnimeList("Ervelan")
                .HasUserAnimeList("SonMati")
                .HasAllAnimes()
                .Build();
        }

        private static MyAnimeListService GetMyAnimeListService(Mock<JikanDotNet.IJikan> jikanMock)
        {
            var listExtractorMock = new ListExtractor(jikanMock.Object);
            var myAnimeListService = new MyAnimeListService(listExtractorMock);
            return myAnimeListService;
        }

        private static Mock<ISeriesRepository> GetSeriesRepository()
        {
            var seriesRepository = new Mock<ISeriesRepository>();
            PrepareDateALiveFranchise(seriesRepository);
            seriesRepository.SetReturnsDefault(false);
            return seriesRepository;
        }

        private static void PrepareDateALiveFranchise(Mock<ISeriesRepository> seriesRepository)
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

        private static FranchiseService GetFranchiseService(Mock<JikanDotNet.IJikan> jikanMock)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(JikanAnimeProvider))).Returns(new JikanAnimeProvider(jikanMock.Object));
            serviceProvider.Setup(s => s.GetService(typeof(JikanMangaProvider))).Returns(new JikanMangaProvider(jikanMock.Object));
            var franchiseService = new FranchiseService(serviceProvider.Object, new FranchiseAssembler(new RelationService()));
            return franchiseService;
        }
    }
}
