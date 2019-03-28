using AniCharades.Data.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniCharades.Data.Models
{
    public class RelationCriteria
    {
        [Key]
        public long Id { get; set; }

        [JsonProperty("keywords")]
        public string[] Keywords { get; set; }

        [JsonProperty("keywordsMatch")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeywordMatch KeywordsMatch { get; set; }

        [JsonProperty("relations")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RelationType[] Relations { get; set; }

        [JsonProperty("strategy")]
        public string Strategy { get; set; }
    }
}
