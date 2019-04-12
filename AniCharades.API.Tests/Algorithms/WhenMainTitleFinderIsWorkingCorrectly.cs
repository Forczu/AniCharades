using AniCharades.Algorithms.Franchise;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AniCharades.API.Tests.Algorithms
{
    public class WhenMainTitleFinderIsWorkingCorrectly : BaseTest
    {
        Dictionary<string, string[]> TitleCollections = new Dictionary<string, string[]>();

        public WhenMainTitleFinderIsWorkingCorrectly()
        {
            var titleCollections = Config.GetSection("Jikan:Titles").GetChildren().Select(c => c.Key).ToArray();
            foreach (var titleCollection in titleCollections)
            {
                var titles = Config.GetSection($"Jikan:Titles:{titleCollection}").Get<string[]>();
                TitleCollections.Add(titleCollection, titles); 
            }
        }

        [Theory]
        [InlineData("Kakegurui", "Kakegurui")]
        [InlineData("MobPsycho", "Mob Psycho 100")]
        [InlineData("SenkanYamato", "Uchuu Senkan Yamato")]
        [InlineData("Arslan", "Arslan Senki")]
        [InlineData("Bakuman", "Bakuman.")]
        [InlineData("Berserk", "Berserk")]
        [InlineData("Danganronpa", "Danganronpa")]
        [InlineData("Neptunia", "Choujigen Game Neptune")]
        [InlineData("DeathBilliards", "Death Billiards")]
        [InlineData("ef", "ef: A Tale of Memories.")]
        [InlineData("FullMetalPanic", "Full Metal Panic!")]
        [InlineData("GochiUsa", "Gochuumon wa Usagi Desu ka?")]
        [InlineData("HajimeNoIppo", "Hajime no Ippo")]
        public void CollectionShouldHaveExpectedMainTitle(string franchiseKey, string expectedTitle)
        {
            // given
            var titles = TitleCollections[franchiseKey];
            // when
            var title = MainTitleFinder.GetMainTitle(titles);
            // then
            Assert.Equal(expectedTitle, title);
        }
    }
}
