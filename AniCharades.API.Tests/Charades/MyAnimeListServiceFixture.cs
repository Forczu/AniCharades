using AniCharades.Algorithms.MyAnimeList;
using AniCharades.API.Tests.LargeMocks;
using AniCharades.Services.Implementation;
using AniCharades.Services.Interfaces;
using JikanDotNet;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.API.Tests.Charades
{
    public class MyAnimeListServiceFixture : IDisposable
    {
        public IMyAnimeListService Object { get; private set; }

        public MyAnimeListServiceFixture()
        {
            var jikanMock = new JikanMockBuilder()
                .HasUserAnimeList("Ervelan")
                .HasUserAnimeList("SonMati")
                .HasUserMangaList("Onrix")
                .HasAllAnimes()
                .Build();
            var listExtractorMock = new ListExtractor(jikanMock.Object);
            Object = new MyAnimeListService(listExtractorMock);
        }

        public void Dispose()
        {
            Object = null;
        }
    }
}
