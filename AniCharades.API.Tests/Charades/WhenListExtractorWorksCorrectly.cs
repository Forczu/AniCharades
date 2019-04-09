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
        public void MergedListEntriesShouldHaveBothUsers(string title)
        {
            // given
            string[] usernames = { "Ervelan", "SonMati" };
            // when
            var mergedList = fixture.Object.GetMergedAnimeLists(usernames).Result;
            // then
            Assert.True(mergedList.First(e => e.Title == title).Users.All(u => usernames.Contains(u)));
        }
    }
}
