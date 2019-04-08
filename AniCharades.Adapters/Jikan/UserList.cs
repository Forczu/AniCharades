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

        public UserList(ICollection<IListEntry> entries)
        {
            Entries = entries;
        }

        public UserList(ICollection<AnimeListEntry> entries)
        {
            Entries = entries.Select(e => new JikanAnimeListEntryAdapter(e)).Cast<IListEntry>().ToList();
        }

        public UserList(ICollection<MangaListEntry> entries)
        {
            Entries = entries.Select(e => new JikanMangaListEntryAdapter(e)).Cast<IListEntry>().ToList();
        }
    }
}
