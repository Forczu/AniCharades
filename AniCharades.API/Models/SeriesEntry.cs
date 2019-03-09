using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Models
{
    public class SeriesEntry
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int[] AnimePositions { get; set; }

        public int[] MangaPositions { get; set; }

        public string ImageUrl { get; set; }

        public string[] Translations { get; set; }
    }
}
