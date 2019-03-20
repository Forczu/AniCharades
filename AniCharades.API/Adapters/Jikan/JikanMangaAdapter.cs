using AniCharades.API.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Adapters.Jikan
{
    public class JikanMangaAdapter : IEntryInstance
    {
        private readonly Manga manga;

        public long Id => manga.MalId;

        public string Title => manga.Title;

        public string Translation => manga.TitleEnglish;

        public ICollection<string> Synonyms => manga.TitleSynonyms;

        public string ImageUrl => manga.ImageURL;

        public JikanMangaAdapter(Manga manga)
        {
            this.manga = manga;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var otherManga = obj as JikanMangaAdapter;
            if (otherManga == null)
                return false;
            return Id == otherManga.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
