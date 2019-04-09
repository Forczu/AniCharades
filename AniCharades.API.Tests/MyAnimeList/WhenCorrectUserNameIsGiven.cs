using AniCharades.Algorithms.MyAnimeList;
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
            var listExtractorMock = new ListExtractor(jikanMock.Object);
            myAnimeListService = new MyAnimeListService(listExtractorMock);
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
