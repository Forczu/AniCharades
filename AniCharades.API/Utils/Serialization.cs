using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Utils
{
    public static class Serialization
    {
        /// <summary>
        /// Helper function for converting data from JSON format to ORM entites.
        /// </summary>
        /// <typeparam name="T">Target entity type.</typeparam>
        /// <param name="jsonFile"></param>
        public static ICollection<T> JsonToCollection<T>(string jsonFile)
        {
            var series = JsonConvert.DeserializeObject<ICollection<T>>(jsonFile);
            return series;
        }

        /// <summary>
        /// Helper function for converting data from ORM entites to JSON format.
        /// </summary>
        /// <typeparam name="T">Target entity type.</typeparam>
        /// <param name="jsonFile"></param>
        public static void SaveCollectionToJson<T>(ICollection<T> entities, string outputFile)
        {
            var json = JsonConvert.SerializeObject(entities, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            File.WriteAllText(outputFile, json);
        }
    }
}
