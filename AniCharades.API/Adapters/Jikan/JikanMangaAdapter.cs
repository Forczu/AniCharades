using AniCharades.API.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Adapters.Jikan
{
    public class JikanMangaAdapter : Manga, IEntryInstance
    {
        public long Id => MalId;

        public new string Title => base.Title;

        public string Translation => TitleEnglish;

        public ICollection<string> Synonyms => TitleSynonyms;

        public string ImageUrl => ImageURL;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var otherManga = obj as Manga;
            if (otherManga == null)
                return false;
            return Id == otherManga.MalId;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
