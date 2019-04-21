using System;
using System.Linq;
using Xunit;

namespace AniCharades.API.Tests.Charades
{
    public class WhenListExtractorWorksCorrectly : IClassFixture<MyAnimeListServiceFixture>
    {
        private readonly MyAnimeListServiceFixture fixture;

        public WhenListExtractorWorksCorrectly(MyAnimeListServiceFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("Cardcaptor Sakura")]
        [InlineData("Seto no Hanayome")]
        public void SharedEntriesShouldHaveBothUsers(string title)
        {
            // given
            string[] usernames = { "Ervelan", "SonMati" };
            // when
            var mergedList = fixture.Object.GetMergedAnimeLists(usernames).Result;
            // then
            var entry = mergedList.First(e => e.Title == title);
            Assert.True(entry.Users.All(u => usernames.Contains(u)));
        }

        [Theory]
        [InlineData("Mobile Suit Gundam")]
        [InlineData("Owarimonogatari")]
        public void SeparateEntriesShouldHaveBothUsers(string title)
        {
            // given
            string[] usernames = { "Ervelan", "SonMati" };
            // when
            var mergedList = fixture.Object.GetMergedAnimeLists(usernames).Result;
            // then
            var entry = mergedList.First(e => e.Title == title);
            Assert.True(entry.Users.Contains("Ervelan"));
            Assert.False(entry.Users.Contains("SonMati"));
        }

        [Theory]
        [InlineData("Fairy Tail")]
        public void OnHoldEntriesShouldBeAdded(string title)
        {
            // given
            string[] usernames = { "Ervelan", "SonMati" };
            // when
            var mergedList = fixture.Object.GetMergedAnimeLists(usernames).Result;
            // then
            var entry = mergedList.First(e => e.Title == title);
            Assert.True(entry.Users.All(u => usernames.Contains(u)));
        }

        [Fact]
        public void ListWithRestrictedAccessShouldReturnEmptyList()
        {
            // given
            string[] usernames = { "Onrix" };
            // when
            var mergedList = fixture.Object.GetMergedMangaLists(usernames).Result;
            // then
            Assert.True(mergedList.Count == 0);
        }
    }
}
