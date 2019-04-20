using AniCharades.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Adapters.Jikan
{
    public class JikanMangaAdapter : IEntryInstance
    {
        private readonly Manga manga;
        private JikanRelatedMangaAdapter relatedMangaAdapter;

        public long Id => manga.MalId;

        public string Title => manga.Title;

        public string Translation => manga.TitleEnglish;

        public ICollection<string> Synonyms => manga.TitleSynonyms;

        public string ImageUrl => manga.ImageURL;

        public IRelatedInstance Related => relatedMangaAdapter;

        public TimePeriod TimePeriod => manga.Published;

        public string Type => manga.Type;

        public string Duration => string.Empty;

        public string Description => manga.Synopsis ?? string.Empty;

        public int Episodes
        {
            get
            {
                var parsed = int.TryParse(manga.Chapters, out var chapters);
                return parsed ? chapters : 0;
            }
        }

        public JikanMangaAdapter(Manga manga)
        {
            this.manga = manga;
            this.relatedMangaAdapter = new JikanRelatedMangaAdapter(manga?.Related);
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

        public override string ToString()
        {
            return $"Id: {Id}, Title: {Title}, Type: {Type}";
        }
    }
}
