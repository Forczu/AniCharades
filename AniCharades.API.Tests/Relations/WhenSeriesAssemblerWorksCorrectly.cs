using AniCharades.API.Tests.LargeMocks;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Franchise;
using AniCharades.Services.Implementation;
using AniCharades.Services.Providers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AniCharades.API.Tests.Relations
{
    public class WhenSeriesAssemblerWorksCorrectly
    {
        private readonly FranchiseAssembler seriesAssembler;
        private readonly IEntryProvider entryProvider;

        public WhenSeriesAssemblerWorksCorrectly()
        {
            var jikanMock = new JikanMockBuilder().HasAllAnimes().Build();
            entryProvider = new JikanAnimeProvider(jikanMock.Object);
            seriesAssembler = new FranchiseAssembler();
        }

        [Fact]
        public void NyarukoFranchiseShouldHaveAllTitlesFor2018()
        {
            // given
            long nyarukoId = 11785;
            // when
            var nyarukoSeries = seriesAssembler.Assembly(nyarukoId, entryProvider);
            // then
            Assert.Equal(8, nyarukoSeries.Count);
        }
    }
}
