﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniCharades.Data.Models
{
    public class CharadesEntry
    {
        public CharadesEntry()
        {
            KnownBy = new HashSet<string>();
        }

        public SeriesEntry Series { get; set; }

        public ICollection<string> KnownBy { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Title: " + Series.Title + '\n');
            sb.Append("Animes: " + Series.AnimePositions.Count + "\n");
            sb.Append(string.Join("\n", Series.AnimePositions.OrderBy(a => a.MalId).Select(a => a.MalId.ToString("D5") + " - " + a.Title)) + '\n');
            sb.Append("Mangas: " + Series.MangaPositions.Count + "\n");
            sb.Append(string.Join("\n", Series.MangaPositions.OrderBy(a => a.MalId).Select(a => a.MalId.ToString("D6") + " - " + a.Title)) + '\n');
            sb.Append("Known by: " + string.Join(", ", KnownBy) + "\n");
            sb.Append("Translation: " + Series.Translation.EnglishOfficial);
            return sb.ToString();
        }
    }
}
