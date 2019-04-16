using AniCharades.Data.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AniCharades.Data.Models
{
    public class RelationCriteria
    {
        public long Id { get; set; }

        [JsonProperty("keywords")]
        public string[] Keywords { get; set; }

        [JsonProperty("keywordsMatch")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeywordMatch? KeywordsMatch { get; set; }

        [JsonProperty("relations")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RelationType[] Relations { get; set; }

        [JsonProperty("strategies")]
        public string[] Strategies { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }
}
