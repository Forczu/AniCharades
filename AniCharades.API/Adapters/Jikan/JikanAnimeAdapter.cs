using AniCharades.API.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Adapters.Jikan
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
    }
}
