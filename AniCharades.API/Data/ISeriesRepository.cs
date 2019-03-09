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
    }
}
