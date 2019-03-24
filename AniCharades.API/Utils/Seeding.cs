using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Utils
{
    public static class Seeding
    {
        /// <summary>
        /// Helper for filling a database with series from old JSON file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="seriesRepository"></param>
        public static void FillDatabaseFromJson(string filePath, ISeriesRepository seriesRepository)
        {
            var jsonFile = File.ReadAllText(filePath);
            var seriesCollection = Serialization.JsonToEntityCollection<SeriesEntry>(jsonFile);
            foreach (var series in seriesCollection)
            {
                if (!seriesRepository.SeriesExists(series.Id).Result)
                {
                    seriesRepository.Add(series);
                }
            }
        }
    }
}
