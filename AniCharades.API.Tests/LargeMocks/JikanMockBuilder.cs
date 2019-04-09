using AniCharades.Common.Extensions;
using JikanDotNet;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AniCharades.API.Tests.LargeMocks
{
    public class JikanMockBuilder
    {
        private Mock<IJikan> jikanMock = new Mock<IJikan>();

        private readonly IConfigurationRoot config;

        public JikanMockBuilder()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
        }

        public JikanMockBuilder HasAnime(long malId)
        {
            var animePath = config[$"Jikan:Anime:Path"];
            var animeFilePath = string.Format("{0}{1}.json", animePath, malId.ToString("D5"));
            SetAnime(malId, animeFilePath);
            return this;
        }

        public JikanMockBuilder HasAnimes(IEnumerable<long> malIds)
        {
            malIds.ForEach(id => HasAnime(id));
            return this;
        }

        public JikanMockBuilder HasAllAnimes()
        {
            var animePath = config[$"Jikan:Anime:Path"];
            var files = Directory.EnumerateFiles(animePath);
            foreach (var file in files)
            {
                var malId = Convert.ToInt32(Path.GetFileNameWithoutExtension(file));
                SetAnime(malId, file);
            }
            return this;
        }

        public JikanMockBuilder HasUserAnimeList(string username)
        {
            var userDataPath = config["Jikan:DataPath"] + config["Jikan:UserFolder"];

            var fileNameFront = username.ToLower() + "_animelist_";
            var pageNumber = Directory.EnumerateFiles(userDataPath).Count(
                f => Path.GetFileNameWithoutExtension(f).Contains(fileNameFront));
            var filters = UserAnimeListExtension.All;

            for (int index = 1; index <= pageNumber; index++)
            {
                var pageFilePath = string.Format("{0}{1}{2}.json", userDataPath, fileNameFront, index);
                var pageJson = File.ReadAllText(pageFilePath);
                jikanMock.Setup(j => j.GetUserAnimeList(username, filters, index)).ReturnsAsync(
                    JsonConvert.DeserializeObject<UserAnimeList>(pageJson)
                );
            }
            return this;
        }

        public JikanMockBuilder HasUserMangaeList(string username)
        {
            var userDataPath = config["Jikan:DataPath"] + config["Jikan:UserFolder"];

            var fileNameFront = username.ToLower() + "_mangalist_";
            var pageNumber = Directory.EnumerateFiles(userDataPath).Count(
                f => Path.GetFileNameWithoutExtension(f).Contains(fileNameFront));
            var filters = UserMangaListExtension.All;

            for (int index = 1; index <= pageNumber; index++)
            {
                var pageFilePath = string.Format("{0}{1}{2}.json", userDataPath, fileNameFront, index);
                var pageJson = File.ReadAllText(pageFilePath);
                jikanMock.Setup(j => j.GetUserMangaList(username, filters, index)).ReturnsAsync(
                    JsonConvert.DeserializeObject<UserMangaList>(pageJson)
                );
            }
            return this;
        }

        public Mock<IJikan> Build()
        {
            return jikanMock;
        }

        private void SetAnime(long malId, string filePath)
        {
            var animeJson = File.ReadAllText(filePath);
            jikanMock.Setup(j => j.GetAnime(malId)).ReturnsAsync(() =>
            {
                return JsonConvert.DeserializeObject<Anime>(animeJson);
            });
        }
    }
}
