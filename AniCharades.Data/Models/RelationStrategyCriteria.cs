using AniCharades.Data.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniCharades.Data.Models
{
    public class RelationStrategyCriteria
    {
        [Key]
        public long Id { get; set; }

        public string[] Keywords { get; set; }

        public KeywordMatch KeywordsMatch { get; set; }

        public RelationType[] Relations { get; set; }

        public string Strategy { get; set; }
    }
}
