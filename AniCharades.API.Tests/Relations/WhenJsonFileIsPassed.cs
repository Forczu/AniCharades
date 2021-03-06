﻿using AniCharades.Data.Enumerations;
using AniCharades.Services.Franchise.Relations;
using AniCharades.Services.Franchise.Relations.Common;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [InlineData(RelationType.AlternativeSetting, typeof(AnyWordMatchesStrategy))]
        public void ConfigurationShouldContainDataForRelations(RelationType relationType, Type type)
        {
            // given
            var config = RelationConfiguration.Instance;
            // when
            var criteria = config.GetFromRelation(relationType);
            var strategies = criteria.Strategies.Select(s => RelationFactory.Instance.Get(s));
            // then
            Assert.IsType(type, strategies.First());
        }
    }
}
