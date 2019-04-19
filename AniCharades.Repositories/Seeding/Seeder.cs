using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using System.IO;

namespace AniCharades.Repositories.Seeding
{
    public static class Seeder
    {
        /// <summary>
        /// Helper for filling a database with series from old JSON file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="seriesRepository"></param>
        public static void FillDatabaseFromJson(string filePath, ISeriesRepository seriesRepository)
        {
            var jsonFile = File.ReadAllText(filePath);
            var seriesCollection = Common.Serialization.JsonToCollection<SeriesEntry>(jsonFile);
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
