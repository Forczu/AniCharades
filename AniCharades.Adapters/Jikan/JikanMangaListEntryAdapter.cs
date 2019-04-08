using AniCharades.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.Adapters.Jikan
{
    public class JikanMangaListEntryAdapter : IListEntry
    {
        private readonly MangaListEntry mangaListEntry;

        public long Id => mangaListEntry.MalId;

        public ICollection<string> Users { get; set; } = new List<string>();

        public JikanMangaListEntryAdapter(MangaListEntry mangaListEntry)
        {
            this.mangaListEntry = mangaListEntry;
        }

        public JikanMangaListEntryAdapter(MangaListEntry mangaListEntry, string username) : this(mangaListEntry)
        {
            AddUser(username);
        }

        public JikanMangaListEntryAdapter(MangaListEntry mangaListEntry, ICollection<string> usernames) : this(mangaListEntry)
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
