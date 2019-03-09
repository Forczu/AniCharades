using AniCharades.API.Models;
using AniCharades.API.Utils;
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
            string filePath = config["SeriesCollectionFile"];
            string json = System.IO.File.ReadAllText(filePath);
            // when
            var seriesCollection = Utils.Serialization.JsonToEntityCollection<SeriesEntry>(json).ToArray();
            // then
            var accelWorldEntry = seriesCollection[3];
            Assert.Equal("Accel World", accelWorldEntry.Title);
        }
    }
}
