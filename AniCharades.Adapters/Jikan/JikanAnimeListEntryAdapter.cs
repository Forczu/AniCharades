using AniCharades.Adapters.Interfaces;
using JikanDotNet;

namespace AniCharades.Adapters.Jikan
{
    public class JikanAnimeListEntryAdapter : IListEntry
    {
        private readonly AnimeListEntry animeListEntry;

        public long Id => animeListEntry.MalId;

        public JikanAnimeListEntryAdapter(AnimeListEntry animeListEntry)
        {
            this.animeListEntry = animeListEntry;
        }
    }
}
