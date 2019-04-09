using JikanDotNet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AniCharades.Algorithms.MyAnimeList
{
    public interface IListExtractor
    {
        Task<ICollection<AnimeListEntry>> GetFullAnimeList(string username);

        Task<ICollection<MangaListEntry>> GetFullMangaList(string username);
    }
}
