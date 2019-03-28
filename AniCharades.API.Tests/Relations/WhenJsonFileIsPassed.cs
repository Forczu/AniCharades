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
        [InlineData("Haiyore! Nyaruko-san", typeof(NyarukoRelationStrategy))]
        [InlineData("Kami nomi zo shiru sekai", typeof(KamiNomiRelationStrategy))]
        [InlineData("Magical☆Star Kanon 100%", typeof(KamiNomiRelationStrategy))]
        public void ConfigurationShouldContainDataForTitle(string title, Type type)
        {
            // given
            var config = RelationConfiguration.Instance;
            // when
            var criteria = config.Get(title);
            var strategy = RelationFactory.Instance.Create(criteria.Strategy);
            // then
            Assert.IsType(type, strategy);
        }

        [Theory]
        [InlineData(RelationType.Sequel, typeof(NoWordMatchesStrategy))]
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
