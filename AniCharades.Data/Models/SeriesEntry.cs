using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Data.Models
{
    public class SeriesEntry
    {
        public SeriesEntry()
        {
            AnimePositions = new HashSet<AnimeEntry>();
            MangaPositions = new HashSet<MangaEntry>();
        }

        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public virtual ICollection<AnimeEntry> AnimePositions { get; set; }

        public virtual ICollection<MangaEntry> MangaPositions { get; set; }

        public string ImageUrl { get; set; }

        public virtual Translation Translation { get; set; }
    }
}
