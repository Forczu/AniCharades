using System.Collections.Generic;
using AniCharades.Adapters.Interfaces;
using JikanDotNet;

namespace AniCharades.Adapters.Jikan
{
    public class JikanAnimeListEntryAdapter : IListEntry
    {
        private readonly AnimeListEntry animeListEntry;

        public long Id => animeListEntry.MalId;

        public string Title => animeListEntry.Title;

        public ICollection<string> Users { get; set; } = new List<string>();

        public JikanAnimeListEntryAdapter(AnimeListEntry animeListEntry)
        {
            this.animeListEntry = animeListEntry;
        }

        public JikanAnimeListEntryAdapter(AnimeListEntry animeListEntry, string username) : this(animeListEntry)
        {
            AddUser(username);
        }

        public JikanAnimeListEntryAdapter(AnimeListEntry animeListEntry, ICollection<string> usernames) : this(animeListEntry)
        {
            foreach (var username in usernames)
            {
                AddUser(username);
            }
        }

        public void AddUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return;
            Users.Add(username);
        }
    }
}
