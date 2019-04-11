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
        public void Test()
        {
            string[] usernames = { "Ervelan", "SonMati" };
            charadesCompositionService.Charades.StartComposing(usernames);
            int length = charadesCompositionService.Charades.GetMergedPositionsCount();
            for (int i = 0; i < length; i++)
            {
                var charadesEntry = charadesCompositionService.Charades.MakeNextCharadesEntry().Result;
                if (charadesEntry != null)
                    output.WriteLine(charadesEntry.ToString() + "\n\n");
            }
        }
    }
}
