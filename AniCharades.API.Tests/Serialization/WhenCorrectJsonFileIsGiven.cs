using AniCharades.API.Utils;
using AniCharades.Data.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AniCharades.API.Tests.Serialization
{
    public class WhenCorrectJsonFileIsGiven
    {
        private IConfigurationRoot config;

        public WhenCorrectJsonFileIsGiven()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
        }

        [Fact]
        public void SeriesCollectionShouldDeserializeCorrectly()
        {
            // given
            var filePath = config["SeriesCollectionFile"];
            var json = System.IO.File.ReadAllText(filePath);
            // when
            var seriesCollection = Utils.Serialization.JsonToCollection<SeriesEntry>(json).ToArray();
            // then
            var accelWorldEntry = seriesCollection[3];
            Assert.Equal("Accel World", accelWorldEntry.Title);
        }

        [Fact]
        public void SeriesCollectionShouldSerializeCorrectly()
        {
            // given
            var filePath = config["SeriesMockFile"];
            var series = new SeriesEntry[] 
            {
                new SeriesEntry
                {
                    AnimePositions =
                    {
                        new AnimeEntry { MalId = 123,Title = "Bleach" },
                        new AnimeEntry { MalId = 234,Title = "Angel Beats!" },
                    },
                    Id = 12,
                    Translation = new Translation { English = "Bleach x AB!" },
                    Title = "Bleach Beats!"
                }
            };
            // when
            Utils.Serialization.SaveCollectionToJson(series, filePath);
            var json = System.IO.File.ReadAllText(filePath);
            var seriesCollection = Utils.Serialization.JsonToCollection<SeriesEntry>(json).ToArray();
            // then
            var bleachBeats = seriesCollection[0];
            Assert.Equal("Bleach Beats!", bleachBeats.Title);
            Assert.Equal("Bleach x AB!", bleachBeats.Translation.English);
            Assert.Equal(123, bleachBeats.AnimePositions.First(x => x.Title == "Bleach").MalId);
            Assert.Equal("Angel Beats!", bleachBeats.AnimePositions.First(x => x.MalId == 234).Title);
        }
    }
}
