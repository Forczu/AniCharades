using AniCharades.Data.Enumerations;
using AniCharades.Services.Franchise.Relations;
using AniCharades.Services.Franchise.Relations.Common;
using AniCharades.Services.Franchise.Relations.Custom;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AniCharades.API.Tests.Relations
{
    public class WhenJsonFileIsPassed : BaseTest
    {
        [Theory]
        [InlineData(RelationType.Sequel, typeof(AnyWordMatchesStrategy))]
        [InlineData(RelationType.SpinOff, typeof(SpinOffRelationStrategy))]
        [InlineData(RelationType.Summary, typeof(NoWordMatchesStrategy))]
        [InlineData(RelationType.AlternativeSetting, typeof(EveryWordMatchesStrategy))]
        public void ConfigurationShouldContainDataForRelations(RelationType relationType, Type type)
        {
            // given
            var config = RelationConfiguration.Instance;
            // when
            var criteria = config.Get(relationType);
            var strategy = RelationFactory.Instance.Create(criteria.Strategy);
            // then
            Assert.IsType(type, strategy);
        }
    }
}
