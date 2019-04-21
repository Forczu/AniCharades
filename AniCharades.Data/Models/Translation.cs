using Newtonsoft.Json;

namespace AniCharades.Data.Models
{
    public class Translation
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Polish { get; set; }

        public string English { get; set; }

        public string Japanese { get; set; }

        [JsonIgnore]
        public int SeriesId { get; set; }

        [JsonIgnore]
        public virtual SeriesEntry Series { get; set; }
    }
}
