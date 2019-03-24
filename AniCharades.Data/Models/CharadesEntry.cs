using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Data.Models
{
    public class CharadesEntry
    {
        public SeriesEntry Series { get; set; }

        public ICollection<string> KnownBy { get; set; }
    }
}
