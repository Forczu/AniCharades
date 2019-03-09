using AniCharades.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        /// <param name="filePath"></param>
        public static IEnumerable<T> JsonToEntityCollection<T>(string filePath)
        {
            var series = JsonConvert.DeserializeObject<IEnumerable<T>>(filePath);
            return series;
        }
    }
}
