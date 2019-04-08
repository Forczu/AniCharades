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

        public JikanMangaListEntryAdapter(MangaListEntry mangaListEntry)
        {
            this.mangaListEntry = mangaListEntry;
        }
    }
}
