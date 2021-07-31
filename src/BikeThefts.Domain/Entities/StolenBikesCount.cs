using Newtonsoft.Json;

namespace BikeThefts.Domain.Entities
{
    public class StolenBikesCount
    {
        [JsonProperty("Proximity")]
        public int Thefts { get; set; }
    }
}
