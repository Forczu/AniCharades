using AniCharades.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniCharades.Adapters.Jikan
{
    public class UserList
    {
        public ICollection<IListEntry> Entries { get; set; }

        public string Username { get; set; }

        public UserList(ICollection<IListEntry> entries, string username)
        {
            Entries = entries;
            Username = username;
        }

        public UserList(ICollection<AnimeListEntry> entries, string username)
        {
            Entries = entries.Select(e => new JikanAnimeListEntryAdapter(e)).Cast<IListEntry>().ToList();
            Username = username;
        }

        public UserList(ICollection<MangaListEntry> entries, string username)
        {
            Entries = entries.Select(e => new JikanMangaListEntryAdapter(e)).Cast<IListEntry>().ToList();
            Username = username;
        }
    }
}
