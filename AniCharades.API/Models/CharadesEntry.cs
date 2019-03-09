using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Models
{
    public class CharadesEntry
    {
        public SeriesEntry Series { get; set; }

        public int[] KnownBy { get; set; }
    }
}
