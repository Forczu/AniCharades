using AniCharades.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Data
{
    public interface ISeriesRepository
    {
        Task<IEnumerable<SeriesEntry>> Get();

        Task<SeriesEntry> Get(int id);

        Task<bool> SeriesExists(int id);

        Task<bool> SeriesExistsByAnimeId(int id);

        Task<bool> SeriesExistsByMangaId(int id);

        Task Add(SeriesEntry series);

        Task Update(SeriesEntry series);
    }
}
