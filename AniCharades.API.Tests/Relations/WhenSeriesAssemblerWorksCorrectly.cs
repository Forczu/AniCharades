using AniCharades.API.Algorithms.SeriesAssembler;
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
        private readonly AnimeSeriesAssembler animeSeriesAssembler;

        public WhenSeriesAssemblerWorksCorrectly()
        {
            var repoMock = new Mock<IRelationCriteriaRepository>();
            repoMock.SetReturnsDefault(Task.FromResult(new RelationCriteria { Strategy = "nyaruko" }));
            var jikanMock = new JikanMockBuilder().HasAllAnimes().Build();
            animeSeriesAssembler = new AnimeSeriesAssembler(jikanMock.Object, repoMock.Object);
        }

        [Fact]
        public void NyarukoFranchiseShouldHaveAllTitlesFor2018()
        {
            // given
            long nyarukoId = 11785;
            // when
            var nyarukoSeries = animeSeriesAssembler.Assembly(nyarukoId);
            // then
            Assert.Equal(8, nyarukoSeries.Count);
        }
    }
}
