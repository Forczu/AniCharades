using AniCharades.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Adapters.Jikan
{
    public class JikanAnimeAdapter : IEntryInstance
    {
        private readonly Anime anime;
        private JikanRelatedAnimeAdapter relatedAnimeAdapter;

        public long Id => anime.MalId;

        public string Title => anime.Title;

        public string Translation => anime.TitleEnglish;

        public ICollection<string> Synonyms => anime.TitleSynonyms;

        public string ImageUrl => anime.ImageURL;

        public IRelatedInstance Related => relatedAnimeAdapter;

        public TimePeriod TimePeriod => anime.Aired;

        public string Type => anime.Type;

        public string Duration => anime.Duration;

        public string Description => anime.Synopsis ?? string.Empty;

        public JikanAnimeAdapter(Anime anime)
        {
            this.anime = anime;
            this.relatedAnimeAdapter = new JikanRelatedAnimeAdapter(anime.Related);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var otherAnime = obj as JikanAnimeAdapter;
            if (otherAnime == null)
                return false;
            return Id == otherAnime.Id;
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
