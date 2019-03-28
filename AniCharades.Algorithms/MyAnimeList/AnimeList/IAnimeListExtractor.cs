using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Algorithms.MyAnimeList.AnimeList
{
    public interface IAnimeListExtractor
    {
        Task<ICollection<AnimeListEntry>> GetFullList(string username);
    }
}
