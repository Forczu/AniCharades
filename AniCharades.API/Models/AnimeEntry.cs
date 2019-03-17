using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Models
{
    public class AnimeEntry
    {
        [Key]
        public long MalId { get; set; }

        public SeriesEntry Series { get; set; }
    }
}
