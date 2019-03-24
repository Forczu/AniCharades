using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Data.Models
{
    public class SeriesEntry
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public ICollection<AnimeEntry> AnimePositions { get; set; }

        public ICollection<MangaEntry> MangaPositions { get; set; }

        public string ImageUrl { get; set; }

        public string[] Translations { get; set; }
    }
}
