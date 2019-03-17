using AniCharades.API.Algorithms.MyAnimeList.AnimeList;
using AniCharades.API.Algorithms.MyAnimeList.MangaList;
using AniCharades.API.Logic.Implementation;
using AniCharades.API.Logic.Interfaces;
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
    public class WhenCorrectUserNameIsGiven
    {
        private readonly IConfigurationRoot config;
        private readonly IMyAnimeListService myAnimeListService;

        public WhenCorrectUserNameIsGiven()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
            var jikanMock = PrepareJikanMock();
            var animeListExtractorMock = new AnimeListExtractor(jikanMock.Object);
            var mangaListExtractorMock = new MangaListExtractor(jikanMock.Object);
            myAnimeListService = new MyAnimeListService(animeListExtractorMock, mangaListExtractorMock);
        }

        private Mock<IJikan> PrepareJikanMock()
        {
            Mock<IJikan> jikanMock = new Mock<IJikan>();
            PrepareAnimeList(jikanMock);
            PrepareMangaList(jikanMock);
            return jikanMock;
        }

        private void PrepareAnimeList(Mock<IJikan> jikanMock)
        {
            var ervieKey = "Jikan:User:Ervelan:Animelist";
            var filePath = config[ervieKey + ":Path"];
            var pageNubmer = Convert.ToInt32(config[ervieKey + ":PageNumber"]);
            var animeListPageTemplate = config[ervieKey + ":PageNameTemplate"];
            var filters = UserAnimeListExtension.All;
            
            for (int index = 1; index <= pageNubmer; index++)
            {
                var pageFilePath = string.Format("{0}\\{1}{2}.json", filePath, animeListPageTemplate, index);
                var pageJson = File.ReadAllText(pageFilePath);
                jikanMock.Setup(j => j.GetUserAnimeList("Ervelan", filters, index)).ReturnsAsync(
                    JsonConvert.DeserializeObject<UserAnimeList>(pageJson)
                );
            }
        }

        private void PrepareMangaList(Mock<IJikan> jikanMock)
        {
            var ervieKey = "Jikan:User:Ervelan:Mangalist";
            var filePath = config[ervieKey + ":Path"];
            var pageNubmer = Convert.ToInt32(config[ervieKey + ":PageNumber"]);
            var animeListPageTemplate = config[ervieKey + ":PageNameTemplate"];
            var filters = UserMangaListExtension.All;

            for (int index = 1; index <= pageNubmer; index++)
            {
                var pageFilePath = string.Format("{0}\\{1}{2}.json", filePath, animeListPageTemplate, index);
                var pageJson = File.ReadAllText(pageFilePath);
                jikanMock.Setup(j => j.GetUserMangaList("Ervelan", filters, index)).ReturnsAsync(
                    JsonConvert.DeserializeObject<UserMangaList>(pageJson)
                );
            }
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
