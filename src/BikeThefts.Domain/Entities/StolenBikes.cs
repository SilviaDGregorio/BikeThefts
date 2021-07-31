using Newtonsoft.Json;

namespace BikeThefts.Domain.Entities
{
    public class StolenBikes : Filters
    {
        [JsonProperty("Proximity")]
        public int Thefts { get; set; }
    }
}
