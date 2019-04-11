using AniCharades.Algorithms.MyAnimeList;
using AniCharades.API.Tests.LargeMocks;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Franchise;
using AniCharades.Services.Implementation;
using AniCharades.Services.Interfaces;
using AniCharades.Services.Providers;
using Moq;
using System;

namespace AniCharades.API.Tests.Charades
{
    public class CharadesCompositionServiceFixture : IDisposable
    {
        public ICharadesCompositionService Charades { get; private set; }

        public CharadesCompositionServiceFixture()
        {
            var jikanMock = new JikanMockBuilder()
                .HasUserAnimeList("Ervelan")
                .HasUserAnimeList("SonMati")
                .HasAllAnimes()
                .Build();
            var listExtractorMock = new ListExtractor(jikanMock.Object);
            var myAnimeListService = new MyAnimeListService(listExtractorMock);

            var seriesRepository = new Mock<ISeriesRepository>();
            seriesRepository.SetReturnsDefault(false);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(JikanAnimeProvider))).Returns(new JikanAnimeProvider(jikanMock.Object));
            serviceProvider.Setup(s => s.GetService(typeof(JikanMangaProvider))).Returns(new JikanMangaProvider(jikanMock.Object));
            var franchiseService = new FranchiseService(serviceProvider.Object, new FranchiseAssembler(new RelationService()));

            Charades = new CharadesCompositionService(myAnimeListService, seriesRepository.Object, franchiseService);
        }

        public void Dispose()
        {
            Charades = null;
        }
    }
}
