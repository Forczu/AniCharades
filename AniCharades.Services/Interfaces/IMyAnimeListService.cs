using AniCharades.Adapters.Interfaces;
using JikanDotNet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AniCharades.Services.Interfaces
{
    public interface IMyAnimeListService
    {
        Task<ICollection<AnimeListEntry>> GetAnimeList(string username);

        Task<ICollection<MangaListEntry>> GetMangaList(string username);

        Task<ICollection<IListEntry>> GetMergedAnimeLists(ICollection<string> usernames);

        Task<ICollection<IListEntry>> GetMergedMangaLists(ICollection<string> usernames);
    }
}
