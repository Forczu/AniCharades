using AniCharades.Adapters.Jikan;
using AniCharades.API.Algorithms.Franchise;
using AniCharades.API.Tests.LargeMocks;
using JikanDotNet;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AniCharades.API.Tests.Franchise
{
    public class WhenFranchiseAlgorithmsWorkCorrectly
    {
        private static readonly long[] NyarukoEntries = { 7596, 9598, 10477, 11785, 15699, 18713, 23619, 26023 };
        private static readonly long[] LoveLiveEntries = { 9907, 9930, 11033, 12637, 14951, 15051, 19111, 20745,
            20877, 21853, 24997, 25897, 30829, 30896, 31780, 32476, 32481, 32526, 32730, 32953, 33106, 34973, 35006, 36473, 37027 };
        private readonly Mock<IJikan> jikanMock = new JikanMockBuilder()
            .HasAnimes(NyarukoEntries)
            .HasAnimes(LoveLiveEntries)
            .Build();

        [Fact]
        public void NyarukoSeriesShouldHaveFirstTvSeriesSelectedAsMain()
        {
            // given
            var franchiseCreator = new FranchiseCreator();
            var nyarukoAdapters = NyarukoEntries.Select(n => new JikanAnimeAdapter(jikanMock.Object.GetAnime(n).Result)).ToArray();
            // when
            var nyarukoSeries = franchiseCreator.Create(nyarukoAdapters);
            // then
            Assert.Equal("Haiyore! Nyaruko-san", nyarukoSeries.Title);
            Assert.Equal("https://cdn.myanimelist.net/images/anime/6/49081.jpg", nyarukoSeries.ImageUrl);
        }

        [Fact]
        public void LoveLiveSeriesShouldHaveFirstTvSeriesSelectedAsMain()
        {
            // given
            var franchiseCreator = new FranchiseCreator();
            var loveLiveAdapters = LoveLiveEntries.Select(n => new JikanAnimeAdapter(jikanMock.Object.GetAnime(n).Result)).ToArray();
            // when
            var loveLiveSeries = franchiseCreator.Create(loveLiveAdapters);
            // then
            Assert.Equal("Love Live! School Idol Project", loveLiveSeries.Title);
            Assert.Equal("https://cdn.myanimelist.net/images/anime/11/56849.jpg", loveLiveSeries.ImageUrl);
        }
    }
}