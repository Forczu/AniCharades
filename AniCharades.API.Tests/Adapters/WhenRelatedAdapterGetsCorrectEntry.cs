using AniCharades.Adapters.Jikan;
using AniCharades.Data.Enumerations;
using JikanDotNet;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AniCharades.API.Tests.Adapters
{
    public class WhenRelatedAdapterGetsCorrectEntry
    {
        private static readonly int FateAnimeId = 356;

        private Mock<IJikan> jikanMock;
        private readonly IConfigurationRoot config;

        public WhenRelatedAdapterGetsCorrectEntry()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
            PrepareJikanMock();
        }

        private void PrepareJikanMock()
        {
            jikanMock = new Mock<IJikan>();
            var fateKey = "Jikan:Anime:FateStayNight";
            var filePath = config[fateKey + ":Path"];
            var fateJson = File.ReadAllText(filePath);
            jikanMock.Setup(j => j.GetAnime(FateAnimeId)).ReturnsAsync(
                                JsonConvert.DeserializeObject<Anime>(fateJson)
                            );
        }

        [Fact]
        public void ItShouldCreateFullRelatedCollection()
        {
            // given
            var fateAnime = jikanMock.Object.GetAnime(FateAnimeId).Result;
            int ubwMovieId = 6922, fateZeroId = 11741, emiyaGohanId = 37033;
            // when
            var fateAdapter = new JikanAnimeAdapter(fateAnime);
            // then
            var relatedPositions = fateAdapter.Related.AllRelatedPositions;
            Assert.Contains(relatedPositions, r => r.MalId == ubwMovieId && r.RelationType == RelationType.AlternativeVersion);
            Assert.Contains(relatedPositions, r => r.MalId == fateZeroId && r.RelationType == RelationType.Prequel);
            Assert.Contains(relatedPositions, r => r.MalId == emiyaGohanId && r.RelationType == RelationType.SpinOff);
        }
    }
}
