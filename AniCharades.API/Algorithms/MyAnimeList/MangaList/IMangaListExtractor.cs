﻿using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.MyAnimeList.MangaList
{
    public interface IMangaListExtractor
    {
        Task<ICollection<MangaListEntry>> GetFullList(string username);
    }
}