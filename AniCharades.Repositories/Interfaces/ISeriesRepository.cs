using AniCharades.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Repositories.Interfaces
{
    public interface ISeriesRepository
    {
        Task<IEnumerable<SeriesEntry>> Get();

        Task<SeriesEntry> Get(int id);

        Task<SeriesEntry> GetByAnimeId(long id);

        Task<SeriesEntry> GetByMangaId(long id);

        Task<bool> SeriesExists(int id);

        Task<bool> SeriesExistsByAnimeId(long id);

        Task<bool> SeriesExistsByMangaId(long id);

        Task Add(SeriesEntry series);

        Task Update(SeriesEntry series);
    }
}
