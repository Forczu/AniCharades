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
        private readonly Mock<IJikan> jikanMock = new JikanMockBuilder()
            .HasAnimes(NyarukoEntries)
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
    }
}