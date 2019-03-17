using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.API.Logic.Interfaces;
using AniCharades.API.Models;

namespace AniCharades.API.Logic.Implementation
{
    public class CharadesCompositionService : ICharadesCompositionService
    {
        private static readonly IList<CharadesEntry> SampleData = new List<CharadesEntry>()
        {
            new CharadesEntry()
            {
                Series = new SeriesEntry()
                {
                    Id = 1,
                    Title = "Bleach",
                    AnimePositions = new List<AnimeEntry> { new AnimeEntry() { MalId = 123 } },
                    MangaPositions = new List<MangaEntry> { new MangaEntry() { MalId = 234 } },
                    ImageUrl = "bleachImage.png"
                },
                KnownBy = new string[] { "111", "222" }
            },
            new CharadesEntry()
            {
                Series = new SeriesEntry()
                {
                    Id = 1,
                    Title = "Date A Live",
                    AnimePositions = new List<AnimeEntry> { new AnimeEntry() { MalId = 456 } },
                    MangaPositions = new List<MangaEntry> { new MangaEntry() { MalId = 897 } },
                    ImageUrl = "dalImage.png"
                },
                KnownBy = new string[] { "222", "333" }
            }
        };

        public ICollection<CharadesEntry> GetCompositedCharades(IEnumerable<string> usernames)
        {
            return SampleData;
        }
    }
}
