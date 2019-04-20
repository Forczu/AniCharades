﻿using AniCharades.Adapters.Jikan;
using AniCharades.Contracts.Charades;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AniCharades.API.Tests.Charades
{
    public class WhenCharadesCompositionIsConfiguredCorrectly : IClassFixture<CharadesCompositionServiceFixture>
    {
        private readonly CharadesCompositionServiceFixture charadesCompositionService;
        private readonly ITestOutputHelper output;

        public WhenCharadesCompositionIsConfiguredCorrectly(CharadesCompositionServiceFixture charadesCompositionService, ITestOutputHelper output)
        {
            this.charadesCompositionService = charadesCompositionService;
            this.output = output;
        }

        [Fact]
        public async Task NewEntryFromFranchiseShouldbeAddedToExistingFranchiseProvidedByDb()
        {
            // given
            var dateALiveEntries = new[]
            {
                new JikanAnimeListEntryAdapter(new JikanDotNet.AnimeListEntry()
                {
                    MalId = 19163,
                    Title = "Date A Live II"
                }, "SonMati"),
                new JikanAnimeListEntryAdapter(new JikanDotNet.AnimeListEntry()
                {
                    MalId = 36633,
                    Title = "Date A Live III"
                }, "Ervelan")
            };
            // when
            charadesCompositionService.Object.StartComposing(dateALiveEntries, Contracts.Enums.EntrySource.Anime);
            // then
            var firstDalCharades = await charadesCompositionService.Object.MakeNextCharadesEntry();
            var nextDalCharades = await charadesCompositionService.Object.MakeNextCharadesEntry();
            Assert.True(nextDalCharades.Series.AnimePositions.Count == 6);
            Assert.Contains(nextDalCharades.Series.AnimePositions, a => a.Title.Equals("Date A Live"));
            Assert.True(nextDalCharades.KnownBy.Contains("SonMati"));
            Assert.True(nextDalCharades.KnownBy.Contains("Ervelan"));
        }

        [Fact]
        public async Task ExistingEntryFromFranchiseShouldbeAddedToExistingCharades()
        {
            // given
            var dateALiveEntries = new[]
            {
                new JikanAnimeListEntryAdapter(new JikanDotNet.AnimeListEntry()
                {
                    MalId = 36633,
                    Title = "Date A Live III"
                }, "Ervelan"),
                new JikanAnimeListEntryAdapter(new JikanDotNet.AnimeListEntry()
                {
                    MalId = 19163,
                    Title = "Date A Live II"
                }, "Progeusz")
            };
            // when
            charadesCompositionService.Object.StartComposing(dateALiveEntries, Contracts.Enums.EntrySource.Anime);
            // then
            var firstDalCharades = await charadesCompositionService.Object.MakeNextCharadesEntry();
            var nextDalCharades = await charadesCompositionService.Object.MakeNextCharadesEntry();
            Assert.Equal(firstDalCharades.Series.AnimePositions.Count, nextDalCharades.Series.AnimePositions.Count);
            Assert.True(nextDalCharades.Series.AnimePositions.Count == 6);
            Assert.True(nextDalCharades.KnownBy.Contains("Progeusz"));
            Assert.True(nextDalCharades.KnownBy.Contains("Ervelan"));
        }

        [Fact]
        public async Task MergedListShouldContainCorrectlyAssignedUsers()
        {
            // given
            var criteria = new GetCharadesCriteria()
            {
                Usernames = new[] { "Ervelan", "SonMati" },
                Sources = new[] { Contracts.Enums.EntrySource.Anime },
                IncludeKnownAdaptations = false
            };
            // when
            charadesCompositionService.Object.StartComposing(criteria);
            while (!charadesCompositionService.Object.IsFinished())
            {
                await charadesCompositionService.Object.MakeNextCharadesEntry();
            }
            // then
            var charades = charadesCompositionService.Object.GetFinishedCharades();
            var setoHana = charades.First(c => c.Series.Title == "Seto no Hanayome");
            Assert.Contains("SonMati", setoHana.KnownBy);
            Assert.Contains("Ervelan", setoHana.KnownBy);
            var gankutsuou = charades.First(c => c.Series.Title == "Gankutsuou");
            Assert.Contains("Ervelan", gankutsuou.KnownBy);
            Assert.DoesNotContain("SonMati", gankutsuou.KnownBy);
            var blendS = charades.First(c => c.Series.Title == "Blend S");
            Assert.Contains("SonMati", blendS.KnownBy);
            Assert.DoesNotContain("Ervelan", blendS.KnownBy);
        }
    }
}
