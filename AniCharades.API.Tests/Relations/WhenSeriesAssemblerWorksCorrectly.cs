using AniCharades.API.Algorithms.SeriesAssembler;
using AniCharades.API.Algorithms.SeriesAssembler.Providers;
using AniCharades.API.Logic.Implementation;
using AniCharades.API.Tests.LargeMocks;
using AniCharades.Data.Context;
using AniCharades.Data.Enumerations;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        private readonly SeriesAssembler seriesAssembler;
        private readonly IEntryProvider entryProvider;

        public WhenSeriesAssemblerWorksCorrectly()
        {
            var repoMock = new Mock<IRelationCriteriaRepository>();
            repoMock.SetReturnsDefault(Task.FromResult(new RelationCriteria { Strategy = "nyaruko" }));
            var jikanMock = new JikanMockBuilder().HasAllAnimes().Build();
            seriesAssembler = new SeriesAssembler(new RelationService(jikanMock.Object, repoMock.Object));
            entryProvider = new JikanAnimeProvider(jikanMock.Object);
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
