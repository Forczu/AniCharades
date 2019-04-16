using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.Data.Models
{
    public class Translation
    {
        public int Id { get; set; }

        public string Polish { get; set; }

        public string English { get; set; }

        public string Japanese { get; set; }

        public int SeriesId { get; set; }

        public virtual SeriesEntry Series { get; set; }
    }
}
