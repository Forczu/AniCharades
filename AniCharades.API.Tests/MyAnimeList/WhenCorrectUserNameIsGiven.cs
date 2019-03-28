using AniCharades.Algorithms.MyAnimeList.AnimeList;
using AniCharades.Algorithms.MyAnimeList.MangaList;
using AniCharades.API.Tests.LargeMocks;
using AniCharades.Services.Implementation;
using AniCharades.Services.Interfaces;
using JikanDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AniCharades.API.Tests.MyAnimeList
{
    public class WhenCorrectUserNameIsGiven : BaseTest
    {
        private readonly IMyAnimeListService myAnimeListService;

        public WhenCorrectUserNameIsGiven()
        {
            var jikanMock = PrepareJikanMock();
            var animeListExtractorMock = new AnimeListExtractor(jikanMock.Object);
            var mangaListExtractorMock = new MangaListExtractor(jikanMock.Object);
            myAnimeListService = new MyAnimeListService(animeListExtractorMock, mangaListExtractorMock);
        }

        private Mock<IJikan> PrepareJikanMock()
        {
           return new JikanMockBuilder()
                .HasUserAnimeList("Ervelan")
                .HasUserMangaeList("Ervelan")
                .Build();
        }

        [Fact]
        public void ItShouldGetEntrieAnimeList()
        {
            // given
            var username = "Ervelan";
            // when
            var animeList = myAnimeListService.GetAnimeList(username).Result;
            // then
            Assert.Contains(animeList, a => a.Title == "Yuuki Yuuna wa Yuusha de Aru");
        }

        [Fact]
        public void ItShouldGetEntrieMangaList()
        {
            // given
            var username = "Ervelan";
            // when
            var mangaList = myAnimeListService.GetMangaList(username).Result;
            // then
            Assert.Contains(mangaList, m => m.Title == "Yokohama Kaidashi Kikou");
        }
    }
}
